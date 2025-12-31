namespace Modules.IAM.Infrastructure.Persistence;

public sealed class UserRow
{
    public int UserId { get; set; }
    public Guid TenantId { get; set; }
    public string Email { get; set; } = default!;
    public string UsernameAr { get; set; } = default!;
    public string? UsernameEn { get; set; }
    public bool IsActive { get; set; }
    public bool IsSystemAdmin { get; set; }
    public bool IsDeleted { get; set; }
    public bool CanBypassDataScope { get; set; }
    public byte AuthProvider { get; set; }
}
