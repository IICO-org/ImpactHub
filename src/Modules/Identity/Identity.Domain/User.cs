using ImpactHub.ERP.SharedKernel; // لكي يتمكن من رؤية القالب المشترك

namespace Identity.Domain;

// هذا هو كائن المستخدم الذي يمثل جدول iam.Users
public sealed class User : BaseEntity
{
    // الهوية المربوطة بـ Azure (LoginID_Ms)
    public Guid ExternalId { get; private set; }

    // إضافة = null!; تحذف علامات التعجب وتخبر المترجم أن القيمة ستأتي من DB
    public string Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;

    public bool IsActive { get; private set; }
    public bool IsSystemAdmin { get; private set; }

    // الـ Constructor الخاص بالـ Domain (محمي لضمان الجودة - مطلوب لـ EF Core)
    private User() { }

    // وظيفة "Factory Method" لإنشاء مستخدم جديد بشكل آمن (للاستخدام في Application Layer)
    public static User Create(Guid externalId, string username, string email, string displayName, int tenantId)
    {
        return new User
        {
            ExternalId = externalId,
            Username = username,
            Email = email,
            DisplayName = displayName,
            IsActive = true,
            IsSystemAdmin = false
            // TenantId سيتم توريثه من BaseEntity
        };
    }

    // وظيفة لتعطيل المستخدم (Business Behavior)
    public void Deactivate()
    {
        this.IsActive = false;
    }
}