using ImpactHub.ERP.SharedKernel;

namespace ImpactHub.ERP.Portal.Context
{
    public interface ICurrentPortalContext
    {
        UserContext CurrentUser { get; }
        TenantContext CurrentTenant { get; }
    }
}
