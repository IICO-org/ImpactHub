namespace ImpactHub.SharedKernel.Security;

/// <summary>
/// Read-only access to the resolved internal user for the current request.
/// Application/Domain code should depend on this, not on ASP.NET.
/// </summary>
public interface ICurrentUser
{
    Guid? TenantId { get; }
    int? UserId { get; }
    bool IsResolved { get; }
}
