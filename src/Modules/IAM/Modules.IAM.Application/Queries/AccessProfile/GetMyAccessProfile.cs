namespace Modules.IAM.Application.Queries.AccessProfile;

// ---- CONTRACT (what the App Shell consumes) ---- 
//--Access Profile Dto
public sealed record AccessProfileDto(
    int UserId,
    Guid TenantId,
    string Provider,
    string Issuer,
    string SubjectId,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions,
    IReadOnlyList<string> Modules);

// ---- QUERY (request) ----
public sealed record GetMyAccessProfileQuery();
