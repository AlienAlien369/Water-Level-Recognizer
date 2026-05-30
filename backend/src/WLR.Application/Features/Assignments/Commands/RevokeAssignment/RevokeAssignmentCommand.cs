using MediatR;
namespace WLR.Application.Features.Assignments.Commands.RevokeAssignment;
public record RevokeAssignmentCommand(Guid Id) : IRequest<bool>;
