namespace ImpactHub.ERP.Modules.Identity.Infrastructure.Security.Authentication;

/// <summary>
/// تمثل إعدادات الربط مع Azure Entra ID (Azure AD)
/// تم وضعها هنا لأن موديول الهوية هو "المالك" لمنطق الـ Security
/// </summary>
public class AzureAdOptions
{
    public const string SectionName = "AzureAd";

    public string Instance { get; set; } = "https://login.microsoftonline.com/";
    public string Domain { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;

    //Scopes التي يطلبها الـ API من Azure (اختياري حسب الحاجة)
    public string Scopes { get; set; } = string.Empty;
}