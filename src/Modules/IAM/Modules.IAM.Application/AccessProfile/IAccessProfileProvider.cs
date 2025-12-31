namespace Modules.IAM.Application.AccessProfile;

/// <summary>
/// Application port for building the "Access Profile" read model.
/// Infrastructure will implement this using DB + caching.
/// </summary>
public interface IAccessProfileProvider
{
    /// <summary>
    /// Returns the effective access profile for the given internal identity (TenantId + UserId),
    /// including roles, permissions, and tenant-enabled modules/features.
    /// </summary>
    Task<Queries.AccessProfile.AccessProfileDto> GetAsync(
        Guid tenantId,
        int userId,
        string provider,
        string? issuer,
        string subjectId,
        CancellationToken ct);

    /// <summary>
    /// Invalidates cached access profile for the given internal identity.
    /// Call this after any change to AccessAssignment / RolePermission / TenantFeatures.
    /// </summary>
    void Invalidate(Guid tenantId, int userId);
}
