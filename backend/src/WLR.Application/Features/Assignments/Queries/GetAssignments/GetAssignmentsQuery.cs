using MediatR;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Assignments.Queries.GetAssignments;
public record GetAssignmentsQuery(QueryParams Params, Guid? CenterId, Guid? UserId) : IRequest<PaginatedResult<AssignmentDto>>;
