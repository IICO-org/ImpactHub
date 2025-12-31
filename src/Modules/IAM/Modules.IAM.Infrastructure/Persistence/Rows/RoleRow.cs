namespace Modules.IAM.Infrastructure.Persistence;

public sealed class RoleRow
{
    public int RoleId { get; set; }
    public string Code { get; set; } = default!;
}
