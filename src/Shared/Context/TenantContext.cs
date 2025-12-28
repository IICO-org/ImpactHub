namespace ImpactHub.ERP.SharedKernel
{
    public sealed class TenantContext
    {
        public Guid TenantId { get; init; }
        public string TenantCode { get; init; } = default!;
        public string TenantName { get; init; } = default!;
        public bool IsActive { get; init; }
        public string? DomainName { get; init; }
    }
}
