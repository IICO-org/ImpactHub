using Microsoft.AspNetCore.Mvc;

namespace ImpactHub.ERP.WebHost.Security;

/// <summary>
/// Declares a permission requirement on a controller or action.
///
/// Usage:
/// ------
///     [RequiresPermission("USERS.VIEW")]
///
/// Architectural intent:
/// --------------------
/// • This attribute is declarative (what is required)
/// • The enforcement logic lives in RequiresPermissionFilter
/// • Keeps controllers clean and readable
///
/// Why TypeFilterAttribute:
/// -----------------------
/// • Allows dependency injection into the filter
/// • Allows passing runtime arguments (permission code)
/// • Avoids static/global authorization logic
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequiresPermissionAttribute : TypeFilterAttribute
{
    /// <summary>
    /// Creates a permission requirement for the given permission code.
    ///
    /// The permission code must match a value from iam.Permissions.Code
    /// (e.g. USERS.VIEW, ROLES.MANAGE).
    /// </summary>
    public RequiresPermissionAttribute(string permissionCode)
        : base(typeof(RequiresPermissionFilter))
    {
        // This argument is passed to RequiresPermissionFilter constructor
        Arguments = new object[] { permissionCode };
    }
}
