using MediatR;

namespace Identity.Application.Commands.AssignRole;

// غيرنا الاسم لـ AssignRoleCommand وغيرنا الإرجاع لـ int
public sealed record AssignRoleCommand(
    int UserId,
    int RoleId,
    int TenantId,
    int AssignedBy) : IRequest<int>;

