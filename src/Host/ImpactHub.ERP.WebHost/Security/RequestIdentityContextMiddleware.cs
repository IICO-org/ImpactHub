using System.Security.Claims;
using ImpactHub.SharedKernel.Security;

namespace ImpactHub.ERP.WebHost.Security;

public sealed class RequestIdentityContextMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var ric = context.RequestServices.GetRequiredService<RequestIdentityContext>();

        if (context.User?.Identity?.IsAuthenticated == true)
        {
            // IMPORTANT:
            // Claims in the raw JWT (tid/sub/iss) can be mapped to different claim types in ClaimsPrincipal.
            // So we support BOTH forms to avoid "current user not resolved" while token is actually valid.

            var tid =
                context.User.FindFirstValue("tid") ??
                context.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid");

            var iss =
                context.User.FindFirstValue("iss") ??
                context.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/issuer");

            var sub =
                context.User.FindFirstValue("sub") ??
                context.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                context.User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (Guid.TryParse(tid, out var tenantId) &&
                !string.IsNullOrWhiteSpace(iss) &&
                !string.IsNullOrWhiteSpace(sub))
            {
                ric.Set(
                    tenantId: tenantId,
                    provider: "entra",
                    issuer: iss,
                    subjectId: sub);
            }
        }

        await next(context);
    }
}
