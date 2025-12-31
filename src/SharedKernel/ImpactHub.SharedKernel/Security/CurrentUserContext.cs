namespace ImpactHub.SharedKernel.Security;

/// <summary>
/// Per-request internal identity (after IAM resolution).
/// This is the identity your modules use for authorization, RLS, auditing, etc.
/// </summary>
public sealed class CurrentUserContext : ICurrentUser
{
    public Guid? TenantId { get; private set; }
    public int? UserId { get; private set; }

    public bool IsResolved => TenantId.HasValue && UserId.HasValue;

    public void Set(Guid tenantId, int userId)
    {
        TenantId = tenantId;
        UserId = userId;
    }
}
