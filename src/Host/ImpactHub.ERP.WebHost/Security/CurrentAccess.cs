using ImpactHub.SharedKernel.Security;
using Modules.IAM.Application.AccessProfile;

namespace ImpactHub.ERP.WebHost.Security;

/// <summary>
/// WebHost implementation of ICurrentAccess.
/// Uses IAM Access Profile (cached per TenantId+UserId) to evaluate permissions.
/// </summary>
public sealed class CurrentAccess : ICurrentAccess
{
    private readonly ICurrentUser _currentUser;
    private readonly RequestIdentityContext _requestIdentity;
    private readonly IAccessProfileProvider _provider;

    private IReadOnlySet<string>? _permissions;

    public CurrentAccess(
        ICurrentUser currentUser,
        RequestIdentityContext requestIdentity,
        IAccessProfileProvider provider)
    {
        _currentUser = currentUser;
        _requestIdentity = requestIdentity;
        _provider = provider;
    }

    public Guid TenantId => _currentUser.TenantId ?? throw new UnauthorizedAccessException("TenantId not resolved.");
    public int UserId => _currentUser.UserId ?? throw new UnauthorizedAccessException("UserId not resolved.");

    public bool HasPermission(string permissionCode)
    {
        if (string.IsNullOrWhiteSpace(permissionCode)) return false;
        EnsureLoaded();
        return _permissions!.Contains(permissionCode);
    }

    public bool HasAny(params string[] permissionCodes)
    {
        if (permissionCodes is null || permissionCodes.Length == 0) return false;
        EnsureLoaded();
        return permissionCodes.Any(p => !string.IsNullOrWhiteSpace(p) && _permissions!.Contains(p));
    }

    internal async Task PreloadAsync(CancellationToken ct)
    {
        if (_permissions is not null) return;

        var dto = await _provider.GetAsync(
            tenantId: TenantId,
            userId: UserId,
            provider: _requestIdentity.Provider ?? "entra",
            issuer: _requestIdentity.Issuer,
            subjectId: _requestIdentity.SubjectId ?? "",
            ct: ct);

        _permissions = dto.Permissions.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private void EnsureLoaded()
    {
        if (_permissions is not null) return;
        PreloadAsync(CancellationToken.None).GetAwaiter().GetResult();
    }
}
