namespace ImpactHub.SharedKernel.Security;

/// <summary>
/// Represents the resolved authorization context for the *current request*.
///
/// This abstraction answers one question only:
///     "What is the current user allowed to do?"
///
/// Architectural intent:
/// --------------------
/// • Lives in SharedKernel → usable by Application & WebHost
/// • Does NOT depend on ASP.NET, HttpContext, or claims
/// • Backed by IAM Access Profile (roles / permissions / modules)
///
/// Why this exists:
/// ---------------
/// • Avoids sprinkling permission logic across controllers
/// • Decouples authorization checks from transport (HTTP)
/// • Enables consistent enforcement (API, background jobs, future gRPC, etc.)
/// </summary>
public interface ICurrentAccess
{
    /// <summary>
    /// Tenant identifier for the current request.
    /// Guaranteed to be resolved for authenticated requests.
    /// </summary>
    Guid TenantId { get; }

    /// <summary>
    /// Internal UserId for the current request.
    /// Guaranteed to be resolved for authenticated requests.
    /// </summary>
    int UserId { get; }

    /// <summary>
    /// Checks whether the current user has the given permission.
    ///
    /// Permission codes are technical identifiers such as:
    ///     "USERS.VIEW", "ROLES.MANAGE", "PROJECTS.APPROVE"
    /// </summary>
    bool HasPermission(string permissionCode);

    /// <summary>
    /// Checks whether the current user has *any* of the given permissions.
    /// Useful for OR-based authorization scenarios.
    /// </summary>
    bool HasAny(params string[] permissionCodes);
}
