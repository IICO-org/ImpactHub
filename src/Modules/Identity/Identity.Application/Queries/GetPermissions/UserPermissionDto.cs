namespace Identity.Application.Queries.GetPermissions;

public record UserPermissionDto(
    int AssignmentId,
    string RoleCode,
    string RoleNameAr,
    string Status,
    DateTime AssignedAt
);