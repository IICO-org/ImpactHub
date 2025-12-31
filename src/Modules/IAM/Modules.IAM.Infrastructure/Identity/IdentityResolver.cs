using Modules.IAM.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

using Modules.IAM.Infrastructure.Persistence;

namespace Modules.IAM.Infrastructure.Identity;

internal sealed class IdentityResolver : IIdentityResolver
{
    private readonly IamDbContext _db;

    public IdentityResolver(IamDbContext db)
    {
        _db = db;
    }

    public async Task<int?> ResolveUserIdAsync(
        string provider,
        string? issuer,
        string subjectId,
        CancellationToken cancellationToken = default)
    {
        provider = provider.Trim();
        subjectId = subjectId.Trim();
        issuer = issuer?.Trim();

        // IMPORTANT: Include IsActive, otherwise disabled identities still resolve.
        var query = _db.UserIdentities
            .Where(x => x.IsActive
                        && x.Provider == provider
                        && x.SubjectId == subjectId);

        if (issuer is null)
        {
            query = query.Where(x => x.Issuer == null);
        }
        else
        {
            query = query.Where(x => x.Issuer == issuer);
        }

        return await query
            .Select(x => (int?)x.UserId)
            .SingleOrDefaultAsync(cancellationToken);
    }
}
