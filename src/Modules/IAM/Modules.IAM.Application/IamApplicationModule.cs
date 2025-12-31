using Microsoft.Extensions.DependencyInjection;
using Modules.IAM.Application.Queries.AccessProfile;
using Modules.IAM.Application.Queries.ResolveUserId;

namespace Modules.IAM.Application;

public static class IamApplicationModule
{
    public static IServiceCollection AddIamApplication(this IServiceCollection services)
    {
        // Query/Command handlers (manual registration for now)
        services.AddScoped<ResolveUserIdQueryHandler>();
        services.AddScoped<GetMyAccessProfileHandler>();
        return services;
    }
}
