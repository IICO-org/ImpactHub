using ImpactHub.SharedKernel.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Modules.IAM.Api.Debug;
using Modules.IAM.Application;
using Modules.IAM.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["ConnectionStrings:ImpactHubDb"];
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("ImpactHubDb connection string NOT FOUND");
}

// =====================
// AUTHENTICATION (Entra / SaaS-ready)
// =====================
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

// =====================
// Per-request identity contexts (SharedKernel)
// =====================
builder.Services.AddScoped<RequestIdentityContext>();
builder.Services.AddScoped<CurrentUserContext>();
builder.Services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<CurrentUserContext>());

// =====================
// SaaS Gate + ONE-SHOT UserId resolution using ExternalIdentityKey (NO middleware guessing)
// =====================
builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"[JWT] OnAuthenticationFailed: {context.Exception}");
            return Task.CompletedTask;
        },

        OnTokenValidated = async context =>
        {
            Console.WriteLine("[JWT] OnTokenValidated: OK");

            // TenantId: try raw 'tid' then mapped tenantid claim
            var tid =
                context.Principal?.FindFirst("tid")?.Value ??
                context.Principal?.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            // Issuer: use the validated token issuer (most reliable)
            var issuer = context.SecurityToken?.Issuer;

            // For Entra: SubjectId MUST be oid (stable GUID). No sub fallback.
            var oid =
                context.Principal?.FindFirst("oid")?.Value ??
                context.Principal?.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            Console.WriteLine($"[JWT] Extracted: tid='{tid}', iss='{issuer}', oid='{oid}'");

            if (!Guid.TryParse(tid, out var tenantId) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(oid))
            {
                context.Fail("Missing required identity claims (tid/iss/oid).");
                return;
            }

            // Build external identity key (solid contract)
            var externalIdentity = new ExternalIdentityKey(
                Provider: "entra",
                Issuer: issuer!,
                SubjectId: oid!);

            // Tenant allowlist (config gate)
            var allowedTenants = context.HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()
                .GetSection("AzureAd:AllowedTenants")
                .Get<string[]>() ?? Array.Empty<string>();

            if (!allowedTenants.Contains(tenantId.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                context.Fail("Tenant is not allowlisted.");
                return;
            }

            // Resolve internal UserId from iam.UserIdentities (entra + issuer + oid)
            var resolver = context.HttpContext.RequestServices
                .GetRequiredService<Modules.IAM.Application.Abstractions.IIdentityResolver>();

            var userId = await resolver.ResolveUserIdAsync(
                provider: externalIdentity.Provider,
                issuer: externalIdentity.Issuer,
                subjectId: externalIdentity.SubjectId,
                cancellationToken: context.HttpContext.RequestAborted);

            Console.WriteLine($"[JWT] Identity mapping result: userId={(userId.HasValue ? userId.Value.ToString() : "NULL")}");

            if (!userId.HasValue)
            {
                // Fail authentication so controllers never run
                context.Fail("User identity is not mapped in iam.UserIdentities.");
                return;
            }

            // Set internal current user (what application code depends on)
            var currentUser = context.HttpContext.RequestServices.GetRequiredService<CurrentUserContext>();
            currentUser.Set(tenantId, userId.Value);

            // Also set external identity context (useful for auditing)
            var ric = context.HttpContext.RequestServices.GetRequiredService<RequestIdentityContext>();
            ric.Set(tenantId, externalIdentity.Provider, externalIdentity.Issuer, externalIdentity.SubjectId);
        },

        OnChallenge = context =>
        {
            Console.WriteLine($"[JWT] OnChallenge: error='{context.Error}' desc='{context.ErrorDescription}' uri='{context.ErrorUri}'");
            return Task.CompletedTask;
        }
    };
});

// =====================
// CONTROLLERS (Module APIs)
// =====================
builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(IdentityDebugController).Assembly);

// =====================
// IAM MODULES (DDD)
// =====================
builder.Services.AddIamApplication();
builder.Services.AddIamModule(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();
