namespace ImpactHub.SharedKernel.Security;

/// <summary>
/// Per-request identity extracted from the authenticated token.
/// This is NOT the Domain User; it represents the external identity + tenant context.
/// </summary>
public sealed class RequestIdentityContext
{
    public Guid? TenantId { get; private set; }
    public string? Provider { get; private set; }   // e.g. "entra"
    public string? Issuer { get; private set; }     // iss
    public string? SubjectId { get; private set; }  // sub

    public bool IsAuthenticated =>
        TenantId.HasValue && !string.IsNullOrWhiteSpace(SubjectId);

    public void Set(Guid tenantId, string provider, string issuer, string subjectId)
    {
        TenantId = tenantId;
        Provider = provider;
        Issuer = issuer;
        SubjectId = subjectId;
    }
}
