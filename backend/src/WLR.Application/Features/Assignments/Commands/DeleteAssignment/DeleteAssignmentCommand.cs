using MediatR;
namespace WLR.Application.Features.Assignments.Commands.DeleteAssignment;
public record DeleteAssignmentCommand(Guid Id) : IRequest<bool>;
