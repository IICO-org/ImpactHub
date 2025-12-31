using Modules.IAM.Application.Abstractions;

namespace Modules.IAM.Application.Queries.ResolveUserId;

public sealed class ResolveUserIdQueryHandler
{
    private readonly IIdentityResolver _resolver;

    public ResolveUserIdQueryHandler(IIdentityResolver resolver)
    {
        _resolver = resolver;
    }

    public Task<int?> HandleAsync(
        ResolveUserIdQuery query,
        CancellationToken cancellationToken = default)
    {
        // OnlyActive is enforced inside resolver using IsActive, so we keep it simple here.
        return _resolver.ResolveUserIdAsync(
            query.Provider,
            query.Issuer,
            query.SubjectId,
            cancellationToken);
    }
}
