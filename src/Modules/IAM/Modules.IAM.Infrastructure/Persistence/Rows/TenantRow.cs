namespace Modules.IAM.Infrastructure.Persistence;

public sealed class TenantRow
{
    public Guid TenantId { get; set; }
    public string Code { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public bool IsActive { get; set; }
    public string? DomainName { get; set; }
}
