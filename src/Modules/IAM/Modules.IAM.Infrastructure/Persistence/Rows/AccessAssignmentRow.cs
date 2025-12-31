namespace Modules.IAM.Infrastructure.Persistence;

public sealed class AccessAssignmentRow
{
    public int AssignmentId { get; set; }
    public Guid TenantId { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string Status { get; set; } = default!;
    public bool IsDeleted { get; set; }
}
