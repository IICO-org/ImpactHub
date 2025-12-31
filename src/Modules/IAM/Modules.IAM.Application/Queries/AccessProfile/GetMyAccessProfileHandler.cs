using ImpactHub.SharedKernel.Security;
using Modules.IAM.Application.AccessProfile;

namespace Modules.IAM.Application.Queries.AccessProfile;

public sealed class GetMyAccessProfileHandler
{
    private readonly ICurrentUser _currentUser;
    private readonly RequestIdentityContext _requestIdentity;
    private readonly IAccessProfileProvider _accessProfileProvider;

    public GetMyAccessProfileHandler(
        ICurrentUser currentUser,
        RequestIdentityContext requestIdentity,
        IAccessProfileProvider accessProfileProvider)
    {
        _currentUser = currentUser;
        _requestIdentity = requestIdentity;
        _accessProfileProvider = accessProfileProvider;
    }

    public Task<AccessProfileDto> HandleAsync(
        GetMyAccessProfileQuery _,
        CancellationToken cancellationToken = default)
    {
        if (!_currentUser.IsResolved || _currentUser.UserId is null || _currentUser.TenantId is null)
            throw new UnauthorizedAccessException("Current user is not resolved.");

        return _accessProfileProvider.GetAsync(
            tenantId: _currentUser.TenantId.Value,
            userId: _currentUser.UserId.Value,
            provider: _requestIdentity.Provider ?? "entra",
            issuer: _requestIdentity.Issuer,
            subjectId: _requestIdentity.SubjectId ?? "",
            ct: cancellationToken
        );
    }
}
