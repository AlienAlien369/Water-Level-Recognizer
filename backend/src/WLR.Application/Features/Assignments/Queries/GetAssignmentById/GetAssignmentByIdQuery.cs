using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Assignments.Queries.GetAssignmentById;
public record GetAssignmentByIdQuery(Guid Id) : IRequest<AssignmentDto>;
