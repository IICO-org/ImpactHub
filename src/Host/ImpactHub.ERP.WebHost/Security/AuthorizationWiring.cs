using ImpactHub.SharedKernel.Security;
using Microsoft.Extensions.DependencyInjection;

namespace ImpactHub.ERP.WebHost.Security;


/// <summary>
/// Central authorization wiring for ImpactHub WebHost.
///
/// This extension method keeps Program.cs clean and makes
/// authorization setup discoverable and self-contained.
///
/// IMPORTANT:
/// ----------
/// • This is WebHost-only code (depends on ASP.NET Core DI)
/// • Application and Domain must never reference this
/// </summary>
public static class AuthorizationWiring
{
    /// <summary>
    /// Registers authorization-related services used by
    /// permission-based endpoint enforcement.
    /// </summary>
    public static IServiceCollection AddImpactHubAuthorization(this IServiceCollection services)
    {
        // Provides permission evaluation for the current request.
        services.AddScoped<ICurrentAccess, CurrentAccess>();

        // Authorization filter used by [RequiresPermission] attribute.
        //services.AddScoped<RequiresPermissionFilter>();

        return services;
    }
}
