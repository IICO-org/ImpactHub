namespace Modules.IAM.Infrastructure.Persistence;

public sealed class UserIdentityRow
{
    public int UserIdentityId { get; set; }
    public int UserId { get; set; }
    public string Provider { get; set; } = default!;
    public string? Issuer { get; set; }
    public string SubjectId { get; set; } = default!;
    public bool IsActive { get; set; }
}
