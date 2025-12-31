using ImpactHub.SharedKernel.Security;
using Modules.IAM.Application.Abstractions;

namespace ImpactHub.ERP.WebHost.Security;

public sealed class CurrentUserResolverMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var requestIdentity = context.RequestServices.GetRequiredService<RequestIdentityContext>();
        var currentUser = context.RequestServices.GetRequiredService<CurrentUserContext>();

        if (!requestIdentity.IsAuthenticated ||
            requestIdentity.TenantId is null ||
            string.IsNullOrWhiteSpace(requestIdentity.Provider) ||
            string.IsNullOrWhiteSpace(requestIdentity.Issuer) ||
            string.IsNullOrWhiteSpace(requestIdentity.SubjectId))
        {
            await next(context);
            return;
        }

        var resolver = context.RequestServices.GetRequiredService<IIdentityResolver>();

        var userId = await resolver.ResolveUserIdAsync(
            requestIdentity.Provider!,
            requestIdentity.Issuer!,
            requestIdentity.SubjectId!,
            context.RequestAborted);

        if (userId.HasValue)
        {
            currentUser.Set(requestIdentity.TenantId.Value, userId.Value);
        }

        await next(context);
    }
}
