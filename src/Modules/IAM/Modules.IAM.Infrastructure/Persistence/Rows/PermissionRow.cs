namespace Modules.IAM.Infrastructure.Persistence;

public sealed class PermissionRow
{
    public int PermissionId { get; set; }
    public string Code { get; set; } = default!;
}
