using ImpactHub.SharedKernel.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ImpactHub.ERP.WebHost.Security;

/// <summary>
/// Enforces endpoint permission requirements.
/// 401 if user identity is not resolved, 403 if resolved but lacks permission.
/// </summary>
public sealed class RequiresPermissionFilter : IAsyncAuthorizationFilter
{
    private readonly ICurrentUser _currentUser;
    private readonly CurrentAccess _access;
    private readonly string _permission;

    public RequiresPermissionFilter(ICurrentUser currentUser, ICurrentAccess access, string permission)
    {
        _currentUser = currentUser;
        _access = (CurrentAccess)access;
        _permission = permission;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (!_currentUser.IsResolved || _currentUser.UserId is null || _currentUser.TenantId is null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        await _access.PreloadAsync(context.HttpContext.RequestAborted);

        if (!_access.HasPermission(_permission))
            context.Result = new ForbidResult();
    }
}
