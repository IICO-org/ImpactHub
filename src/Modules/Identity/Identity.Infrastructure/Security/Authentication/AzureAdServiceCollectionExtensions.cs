using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

// الـ Namespace يعبر عن الملكية والطبقة والسياق
namespace ImpactHub.ERP.Modules.Identity.Infrastructure.Security.Authentication;

public static class AzureAdServiceCollectionExtensions
{
    public static IServiceCollection AddAzureAdAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));

        return services;
    }
}