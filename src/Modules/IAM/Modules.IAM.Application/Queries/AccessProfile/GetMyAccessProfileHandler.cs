using ImpactHub.SharedKernel.Security;

namespace Modules.IAM.Application.Queries.AccessProfile;

public sealed class GetMyAccessProfileHandler
{
    private readonly ICurrentUser _currentUser;
    private readonly RequestIdentityContext _requestIdentity;

    public GetMyAccessProfileHandler(ICurrentUser currentUser, RequestIdentityContext requestIdentity)
    {
        _currentUser = currentUser;
        _requestIdentity = requestIdentity;
    }

    public Task<AccessProfileDto> HandleAsync(
        GetMyAccessProfileQuery _,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsResolved || _currentUser.UserId is null || _currentUser.TenantId is null)
            throw new UnauthorizedAccessException("Current user is not resolved.");

        // Phase 1 of Access Profile: identity + empty lists.
        // Next iteration will populate Roles/Permissions/Modules from IAM tables (cached).
        var dto = new AccessProfileDto(
            UserId: _currentUser.UserId.Value,
            TenantId: _currentUser.TenantId.Value,
            Provider: _requestIdentity.Provider ?? "entra",
            Issuer: _requestIdentity.Issuer ?? "",
            SubjectId: _requestIdentity.SubjectId ?? "",
            Roles: Array.Empty<string>(),
            Permissions: Array.Empty<string>(),
            Modules: Array.Empty<string>());

        return Task.FromResult(dto);
    }
}
