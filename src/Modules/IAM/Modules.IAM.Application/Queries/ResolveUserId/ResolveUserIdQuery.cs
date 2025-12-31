namespace Modules.IAM.Application.Queries.ResolveUserId;

public sealed record ResolveUserIdQuery(
    string Provider,
    string? Issuer,
    string SubjectId,
    bool OnlyActive = true);
