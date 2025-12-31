namespace Modules.IAM.Infrastructure.Persistence;

public sealed class FeatureRow
{
    public int FeatureId { get; set; }
    public string Code { get; set; } = default!;
}
