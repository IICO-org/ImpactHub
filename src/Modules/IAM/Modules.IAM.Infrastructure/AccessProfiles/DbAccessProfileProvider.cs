using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Modules.IAM.Application.AccessProfile;
using Modules.IAM.Application.Queries.AccessProfile;
using Modules.IAM.Infrastructure.Persistence; // your IamDbContext namespace
using System.Data;

namespace Modules.IAM.Infrastructure.AccessProfile;

/// <summary>
/// Loads the current user's effective authorization "read model":
/// - Roles: iam.AccessAssignment -> iam.Roles
/// - Permissions: iam.AccessAssignment -> iam.RolePermission -> iam.Permissions
/// - Modules: iam.TenantFeatures -> iam.Features (tenant-level enablement)
///
/// This is Infrastructure-only because it depends on EF Core + IMemoryCache.
/// Application layer sees only IAccessProfileProvider.
/// </summary>
internal sealed class DbAccessProfileProvider : IAccessProfileProvider
{
    private readonly IamDbContext _db;
    private readonly IMemoryCache _cache;

    // Keep TTL short because authorization changes are sensitive.
    // We'll add deterministic invalidation hooks in C8-3.
    private static readonly TimeSpan AbsoluteTtl = TimeSpan.FromMinutes(5);

    public DbAccessProfileProvider(IamDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    /// <summary>
    /// Removes cached access profile for a given (tenantId, userId).
    /// Call this after role assignment changes or role-permission changes.
    /// </summary>
    public void Invalidate(Guid tenantId, int userId)
        => _cache.Remove(CacheKey(tenantId, userId));

    // NOTE: Your IAccessProfileProvider signature currently uses int tenantId in earlier draft.
    // In YOUR schema TenantId is uniqueidentifier, so the provider must use Guid.
    // If your interface currently takes int tenantId, FIX IT NOW (this is not cosmetic).
    public async Task<AccessProfileDto> GetAsync(
        Guid tenantId,
        int userId,
        string provider,
        string? issuer,
        string subjectId,
        CancellationToken ct)
    {
        // Cache is per tenant+user as requested.
        // We intentionally do NOT include provider/issuer/subjectId in the cache key
        // because you want it scoped by internal identity (TenantId/UserId).
        // External identity is just metadata in the response.
        var key = CacheKey(tenantId, userId);

        if (_cache.TryGetValue(key, out AccessProfileDto? cached) && cached is not null)
            return cached;

        // Execute DB reads
        var roles = await LoadRolesAsync(tenantId, userId, ct);
        var permissions = await LoadPermissionsAsync(tenantId, userId, ct);

        // Modules are tenant-enabled features (subscription/feature flags).
        // This is SaaS-friendly and aligns with your iam.Features + iam.TenantFeatures tables.
        var modules = await LoadTenantModulesAsync(tenantId, ct);

        var dto = new AccessProfileDto(
            UserId: userId,
            TenantId: tenantId,          // Ensure AccessProfileDto uses Guid TenantId as well.
            Provider: provider,
            Issuer: issuer ?? "",
            SubjectId: subjectId,
            Roles: roles,
            Permissions: permissions,
            Modules: modules
        );

        // Cache with absolute expiration to bound staleness risk.
        _cache.Set(key, dto, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = AbsoluteTtl,
            Priority = CacheItemPriority.High
        });

        return dto;
    }

    private static string CacheKey(Guid tenantId, int userId)
        => $"iam:access-profile:{tenantId:D}:{userId}";

    private async Task<IReadOnlyList<string>> LoadRolesAsync(Guid tenantId, int userId, CancellationToken ct)
    {
        // Only Active assignments and not soft-deleted.
        // Pending/Revoked/Deleted must not grant role membership.
        var roles = await (
            from aa in _db.AccessAssignments
            join r in _db.Roles on aa.RoleId equals r.RoleId
            where aa.TenantId == tenantId
                  && aa.UserId == userId
                  && aa.IsDeleted == false
                  && aa.Status == "Active"
            select r.Code
        )
        .Distinct()
        .OrderBy(x => x)
        .ToListAsync(ct);

        return roles;
    }

    private async Task<IReadOnlyList<string>> LoadPermissionsAsync(Guid tenantId, int userId, CancellationToken ct)
    {
        // Effective permissions:
        // user -> active roles (AccessAssignment) -> RolePermission -> Permissions(Code)
        var perms = await (
            from aa in _db.AccessAssignments
            join rp in _db.RolePermissions on aa.RoleId equals rp.RoleId
            join p in _db.Permissions on rp.PermissionId equals p.PermissionId
            where aa.TenantId == tenantId
                  && aa.UserId == userId
                  && aa.IsDeleted == false
                  && aa.Status == "Active"
            select p.Code
        )
        .Distinct()
        .OrderBy(x => x)
        .ToListAsync(ct);

        return perms;
    }

    private async Task<IReadOnlyList<string>> LoadTenantModulesAsync(Guid tenantId, CancellationToken ct)
    {
        // Tenant enabled modules (feature flags):
        // - IsEnabled = 1
        // - Not expired (ValidUntil is null or >= now)
        // IMPORTANT: you stored ValidUntil as datetime2(3) - check whether it is UTC in your conventions.
        var utcNow = DateTime.UtcNow;

        var modules = await (
            from tf in _db.TenantFeatures
            join f in _db.Features on tf.FeatureId equals f.FeatureId
            where tf.TenantId == tenantId
                  && tf.IsEnabled == true
                  && (tf.ValidUntil == null || tf.ValidUntil >= utcNow)
            select f.Code
        )
        .Distinct()
        .OrderBy(x => x)
        .ToListAsync(ct);

        return modules;
    }
}
