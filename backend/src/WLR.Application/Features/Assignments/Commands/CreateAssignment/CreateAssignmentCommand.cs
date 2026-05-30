using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Assignments.Commands.CreateAssignment;
public record CreateAssignmentCommand(Guid UserId, Guid CenterId, Guid? LocationId, Guid? MotorId, string? Notes) : IRequest<AssignmentDto>;
