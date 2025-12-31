using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.IAM.Application.Abstractions;
using Modules.IAM.Application.AccessProfile;
using Modules.IAM.Infrastructure.Identity;
using Modules.IAM.Infrastructure.Persistence;

namespace Modules.IAM.Infrastructure;

/// <summary>
/// IAM Infrastructure module entrypoint (Composition Root for IAM).
///
/// Architectural role:
/// -------------------
/// • This class is the ONLY place where IAM infrastructure concerns are wired.
/// • WebHost must never register IAM internals directly.
/// • WebHost calls AddIamModule(...) and IAM becomes fully operational.
///
/// Why this matters:
/// -----------------
/// • Preserves modular monolith boundaries
/// • Prevents Infrastructure leakage into Host
/// • Allows IAM to evolve independently
/// </summary>
public static class IamModule
{
    /// <summary>
    /// Registers all IAM infrastructure services:
    /// - Persistence (DbContext)
    /// - Identity resolution services
    /// - Access Profile read-model + caching
    ///
    /// IMPORTANT:
    /// ----------
    /// • This method must remain idempotent and side-effect free.
    /// • Do NOT add ASP.NET-specific concepts here (HttpContext, middleware, controllers).
    /// • Lifetime choices must align with DbContext scope.
    /// </summary>
    public static IServiceCollection AddIamModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ==========================================================
        // Persistence layer (IAM database access)
        // ==========================================================
        // Registers IamDbContext as Scoped:
        // - One DbContext per request
        // - Required for transactional consistency
        // - Prevents cross-request state leakage
        services.AddDbContext<IamDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("ImpactHubDb")));

        // ==========================================================
        // Identity Resolution
        // ==========================================================
        // Resolves external identity (Entra/local) into internal UserId.
        // This service is used during authentication token validation
        // and must stay inside IAM infrastructure.
        services.AddScoped<IIdentityResolver, IdentityResolver>();

        // ==========================================================
        // Access Profile (Authorization Read Model)
        // ==========================================================
        // Access Profile = computed authorization snapshot:
        //   Roles        → from iam.AccessAssignment
        //   Permissions  → via iam.RolePermission
        //   Modules      → via iam.TenantFeatures (SaaS feature flags)
        //
        // Why caching:
        // -------------
        // • Access profile is read on almost every request
        // • Authorization data changes infrequently
        // • Cache is keyed by (TenantId, UserId)
        //
        // TTL is short (minutes) and acts as a safety net.
        // Explicit invalidation will be added on write operations.
        services.AddMemoryCache();

        // Application-facing port (IAccessProfileProvider)
        // is bound to Infrastructure implementation.
        //
        // Why Scoped:
        // -----------
        // • Depends on DbContext (Scoped)
        // • Must respect per-request tenant/user boundaries
        services.AddScoped<IAccessProfileProvider, global::Modules.IAM.Infrastructure.AccessProfile.DbAccessProfileProvider>();
        return services;
    }
}
