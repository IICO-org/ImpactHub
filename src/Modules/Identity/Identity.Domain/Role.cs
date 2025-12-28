using ImpactHub.ERP.SharedKernel;

namespace Identity.Domain;

// يمثل جدول iam.Roles في Azure VM
public sealed class Role : BaseEntity
{
    // الكود التقني الفريد (مثل: ADMIN, EDITOR)
    // نستخدم = null!; لإخبار C# أن القيمة إلزامية وستُملأ من قاعدة البيانات
    public string Code { get; private set; } = null!;

    public string NameEn { get; private set; } = null!;
    public string NameAr { get; private set; } = null!;
    public string DescriptionEn { get; private set; } = null!;
    public string DescriptionAr { get; private set; } = null!;

    // Constructor محمي لضمان الإنشاء الصحيح من قبل EF Core
    private Role() { }

    // وظيفة "Factory Method" لإنشاء دور جديد (إذا احتجت لإضافته برمجياً)
    public static Role Create(string code, string nameEn, string nameAr, string descEn, string descAr)
    {
        return new Role
        {
            Code = code,
            NameEn = nameEn,
            NameAr = nameAr,
            DescriptionEn = descEn,
            DescriptionAr = descAr
        };
    }

    // وظيفة لتحديث بيانات الدور (Business Logic)
    public void UpdateDetails(string nameEn, string nameAr, string descEn, string descAr)
    {
        this.NameEn = nameEn;
        this.NameAr = nameAr;
        this.DescriptionEn = descEn;
        this.DescriptionAr = descAr;

        // ملاحظة لخبير الـ DB: تاريخ التعديل (ModifiedAt) 
        // سيتم تحديثه تلقائياً في الـ DbContext بفضل الـ BaseEntity
    }
}