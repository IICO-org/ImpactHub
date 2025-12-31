namespace Modules.IAM.Infrastructure.Persistence;

public sealed class TenantFeatureRow
{
    public Guid TenantId { get; set; }
    public int FeatureId { get; set; }

    public bool IsEnabled { get; set; }
    public DateTime? ValidUntil { get; set; }
}
