using Identity.Infrastructure.Persistence;
using Identity.Domain; // تأكد من وجود هذا للوصول لـ AccessAssignment
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Commands.AssignRole;

// لاحظ: غيرنا المدخل ليكون AssignRoleCommand والمخرج ليكون int
public class AssignRoleHandler : IRequestHandler<AssignRoleCommand, int>
{
    private readonly IamDbContext _context;

    public AssignRoleHandler(IamDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        // 1. إنشاء الكيان (Entity)
        var assignment = AccessAssignment.Create(
            request.UserId,
            request.RoleId,
            request.TenantId,
            request.AssignedBy);

        // 2. إضافة السجل لقاعدة البيانات في Azure
        _context.AccessAssignments.Add(assignment);
        await _context.SaveChangesAsync(cancellationToken);

        // 3. إرجاع الـ ID الجديد (AssignmentID) لـ Swagger
        return assignment.Id;
    }
}