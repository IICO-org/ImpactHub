using ImpactHub.ERP.SharedKernel;

namespace Identity.Domain;

// يمثل جدول iam.AccessAssignment
// هذا هو "القرار" الذي يربط المستخدم بالدور في سياق Tenant معين
public sealed class AccessAssignment : BaseEntity
{
    // ربط منطقي عبر الـ IDs فقط (التزاماً بـ ADR-001)
    public int UserId { get; private set; }
    public int RoleId { get; private set; }

    // من الذي اتخذ هذا القرار؟
    public int AssignedBy { get; private set; }

    // حالة التفعيل (Active, Revoked)
    // نستخدم = null!; لإسكات تحذير المترجم لأننا نضمن ملأها في الـ Create أو عبر EF
    public string Status { get; private set; } = null!;

    // Constructor محمي لـ Entity Framework Core
    private AccessAssignment() { }

    // وظيفة إنشاء Assignment جديد مع التحقق من القواعد
    public static AccessAssignment Create(int userId, int roleId, int tenantId, int assignedBy)
    {
        return new AccessAssignment
        {
            UserId = userId,
            RoleId = roleId,
            TenantId = tenantId,
            AssignedBy = assignedBy,
            Status = "Active", // الحالة الابتدائية عند أول Insert
            CreatedAt = DateTime.UtcNow
        };
    }

    // وظيفة سحب الصلاحية (Business Behavior)
    public void Revoke(int revokedBy) // أضفنا باراميتر هنا
    {
        this.Status = "Revoked";
        this.ModifiedAt = DateTime.UtcNow; // مسموح لها لأنها ابنة BaseEntity
        this.ModifiedBy = revokedBy;       // مسموح لها لأنها ابنة BaseEntity
    }
}


