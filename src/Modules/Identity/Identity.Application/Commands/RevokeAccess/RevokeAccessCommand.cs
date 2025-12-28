using MediatR;

namespace Identity.Application.Commands.RevokeAccess;

public record RevokeAccessCommand(int AssignmentId, int RevokedBy) : IRequest<bool>;