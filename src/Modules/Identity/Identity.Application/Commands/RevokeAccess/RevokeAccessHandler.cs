using Identity.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Commands.RevokeAccess;

public class RevokeAccessHandler : IRequestHandler<RevokeAccessCommand, bool>
{
    private readonly IamDbContext _context;
    public RevokeAccessHandler(IamDbContext context) => _context = context;

    public async Task<bool> Handle(RevokeAccessCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _context.AccessAssignments
            .FirstOrDefaultAsync(x => x.Id == request.AssignmentId, cancellationToken);

        if (assignment == null) return false;

        assignment.Revoke(request.RevokedBy);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}