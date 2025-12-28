namespace ImpactHub.ERP.SharedKernel;

// هذا الكائن هو "القالب" لكل الجداول في النظام
public abstract class BaseEntity
{
    // المعرف الأساسي (Primary Key) الموحد
    // يتوافق مع الـ Identity (1,1) في SQL Server
    public int Id { get; protected set; }

    // عزل البيانات (Multi-tenancy) 
    // ADR-001: نستخدم ID فقط لضمان استقلالية كل موديول
    public int TenantId { get; protected set; }

    // أعمدة الرقابة (Audit Columns)
    // نستخدم INT لـ CreatedBy ليرتبط بـ UserID في قاعدة البيانات
    public DateTime CreatedAt { get; protected set; }
    public int CreatedBy { get; protected set; }

    public DateTime? ModifiedAt { get; protected set; }
    public int? ModifiedBy { get; protected set; }
    public bool IsDeleted { get; set; }
}


