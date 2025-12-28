using Identity.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Queries.GetPermissions;

public record GetUserPermissionsQuery(int UserId) : IRequest<List<UserPermissionDto>>;

public class GetUserPermissionsHandler : IRequestHandler<GetUserPermissionsQuery, List<UserPermissionDto>>
{
    private readonly IamDbContext _context;

    public GetUserPermissionsHandler(IamDbContext context) => _context = context;

    public async Task<List<UserPermissionDto>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
    {
        // استعلام LINQ يقوم بعمل Join خلف الكواليس بين الجداول في Azure VM
        return await (from assignment in _context.AccessAssignments
                      join role in _context.Roles on assignment.RoleId equals role.Id
                      where assignment.UserId == request.UserId && !assignment.IsDeleted
                      select new UserPermissionDto(
                          assignment.Id,
                          role.Code,
                          role.NameAr,
                          assignment.Status,
                          assignment.CreatedAt
                      )).ToListAsync(cancellationToken);
    }
}