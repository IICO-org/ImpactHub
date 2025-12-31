namespace Modules.IAM.Application.Abstractions;

public interface IIdentityResolver
{
    Task<int?> ResolveUserIdAsync(
        string provider,
        string? issuer,
        string subjectId,
        CancellationToken cancellationToken = default);
}
