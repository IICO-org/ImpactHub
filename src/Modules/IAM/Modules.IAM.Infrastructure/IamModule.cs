using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.IAM.Application.Abstractions;
using Modules.IAM.Infrastructure.Identity;
using Modules.IAM.Infrastructure.Persistence;

namespace Modules.IAM.Infrastructure;

public static class IamModule
{
    public static IServiceCollection AddIamModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Persistence (kept inside the module)
        services.AddDbContext<IamDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("ImpactHubDb")));

        // Services (kept inside the module)
        services.AddScoped<IIdentityResolver, IdentityResolver>();

        return services;
    }
}
