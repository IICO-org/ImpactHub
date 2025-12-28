using Identity.Api;
using Identity.Application;
using Identity.Infrastructure;
using ImpactHub.ERP.Modules.Identity.Infrastructure.Security.Authentication;

// ------------------------------------------------------------
// 🚨 REQUIRED NAMESPACES TO FIX YOUR ERRORS
// ------------------------------------------------------------
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http; // <-- Needed for Results.Redirect
using Microsoft.AspNetCore.Hosting; // <-- Needed for IsDevelopment
using Microsoft.AspNetCore.Mvc; // <-- Controllers / Razor Pages
using Microsoft.Extensions.DependencyInjection; // <-- AddControllers, AddSwaggerGen, AddEndpointsApiExplorer
using Microsoft.Extensions.Hosting; // <-- IHostEnvironment / middleware
using Microsoft.OpenApi.Models; // <-- Swagger

using System.Security.Claims;
using System.Threading.Tasks;


// ==========================================================
// 1) CREATE WEB APPLICATION BUILDER
// ==========================================================
var builder = WebApplication.CreateBuilder(args);


// ==========================================================
// 2) REGISTER SERVICES (Dependency Injection)
// ==========================================================

// 🔐 API Authentication via Azure AD (Bearer tokens)
builder.Services.AddAzureAdAuthentication(builder.Configuration);

// 🧠 Application layer — MediatR handlers
builder.Services.AddIdentityApplication();

// 🏗️ Infrastructure layer — DbContext, Repos, etc.
builder.Services.AddIdentityInfrastructure(builder.Configuration);

// 🌐 Load controllers from Identity.Api
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Identity.Api.Controllers.AccessController).Assembly);

// 🧭 Razor Pages (UI entry) — allow anonymous login page
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Login");
});


// ==========================================================
// 2.6 SWAGGER — WITH BEARER AUTH SUPPORT
// ==========================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ImpactHub ERP API",
        Version = "v1"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste JWT token here. Format: Bearer {token}"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// ==========================================================
// 3) AUTHENTICATION MODES
// ==========================================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    options.Authority = "https://login.microsoftonline.com/common/v2.0";
    options.ClientId = builder.Configuration["AzureAd:ClientId"];
    options.ClientSecret = builder.Configuration["AzureAd:ClientSecret"];

    options.ResponseType = "code";
    options.SaveTokens = true;
    options.CallbackPath = "/signin-oidc";

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.GetClaimsFromUserInfoEndpoint = true;
    options.TokenValidationParameters.ValidateIssuer = false;

    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = ctx =>
        {
            var identity = (ClaimsIdentity)ctx.Principal!.Identity!;

            var tid = ctx.Principal.FindFirst("tid")?.Value;
            var oid = ctx.Principal.FindFirst("oid")?.Value;

            if (tid != null) identity.AddClaim(new Claim("tenant_id", tid));
            if (oid != null) identity.AddClaim(new Claim("user_oid", oid));

            return Task.CompletedTask;
        }
    };
});


// ==========================================================
// 4) AUTHORIZATION POLICY
// ==========================================================
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});


// ==========================================================
// 5) BUILD APP
// ==========================================================
var app = builder.Build();


// ==========================================================
// 6) PIPELINE MIDDLEWARE
// ==========================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


// 🧭 Razor Pages FIRST (so /login works)
app.MapRazorPages();

// 🌐 Controllers AFTER
app.MapControllers();
// 🚪 /portal should redirect to running Portal app
app.MapGet("/portal", () => Results.Redirect("https://localhost:7240/"));

// 🏠 root → /portal
app.MapGet("/", () => Results.Redirect("/portal"));

app.Run();
