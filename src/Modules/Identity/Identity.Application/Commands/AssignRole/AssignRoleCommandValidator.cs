using FluentValidation;

namespace Identity.Application.Commands.AssignRole;

// تأكد أن الاسم هنا مطابق تماماً للـ record الجديد
public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand>
{
    public AssignRoleCommandValidator()
    {
        // القواعد الشكلية (Guard Rules) - لا تلمس قاعدة البيانات

        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("رقم المستخدم غير صحيح");

        RuleFor(x => x.RoleId)
            .GreaterThan(0)
            .WithMessage("رقم الدور غير صحيح");

        RuleFor(x => x.TenantId)
            .GreaterThan(0)
            .WithMessage("رقم الـ Tenant مطلوب لضمان عزل البيانات");

        RuleFor(x => x.AssignedBy)
            .GreaterThan(0)
            .WithMessage("يجب تحديد هوية الشخص المسؤول عن هذا الإسناد");
    }
}