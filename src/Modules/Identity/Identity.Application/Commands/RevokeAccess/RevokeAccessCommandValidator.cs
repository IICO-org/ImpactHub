using FluentValidation;

namespace Identity.Application.Commands.RevokeAccess;

public class RevokeAccessCommandValidator : AbstractValidator<RevokeAccessCommand>
{
    public RevokeAccessCommandValidator()
    {
        RuleFor(x => x.AssignmentId)
            .NotEmpty().WithMessage("رقم العملية مطلوب للإلغاء");

        RuleFor(x => x.RevokedBy)
            .GreaterThan(0).WithMessage("يجب تحديد من قام بعملية الإلغاء");
    }
}