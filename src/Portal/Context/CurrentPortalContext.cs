using ImpactHub.ERP.SharedKernel;
using Microsoft.AspNetCore.Http;
using System;

namespace ImpactHub.ERP.Portal.Context
{
    public sealed class CurrentPortalContext : ICurrentPortalContext
    {
        private readonly IHttpContextAccessor _accessor;

        public CurrentPortalContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public UserContext CurrentUser => ResolveUser();
        public TenantContext CurrentTenant => ResolveTenant();

        private UserContext ResolveUser()
        {
            var user = _accessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                return new UserContext { IsActive = false };

            return new UserContext
            {
                UserId = 0, // TBD: fill when DB joined
                AzureObjectId = Guid.Parse(user.FindFirst("oid")!.Value),
                Email = user.FindFirst("email")!.Value,
                IsActive = true,
                IsSystemAdmin = false
            };
        }

        private TenantContext ResolveTenant()
        {
            var user = _accessor.HttpContext?.User;

            return new TenantContext
            {
                TenantId = Guid.Parse(user.FindFirst("tenant_id")!.Value),
                TenantCode = "IICO",
                TenantName = "ImpactHub ERP",
                IsActive = true
            };
        }
    }
}
