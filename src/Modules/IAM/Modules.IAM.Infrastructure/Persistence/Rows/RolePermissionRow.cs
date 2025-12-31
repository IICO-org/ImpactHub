namespace Modules.IAM.Infrastructure.Persistence;

public sealed class RolePermissionRow
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }
}
