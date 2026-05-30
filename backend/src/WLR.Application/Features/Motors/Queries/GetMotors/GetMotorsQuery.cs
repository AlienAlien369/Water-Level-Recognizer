using MediatR;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Motors.Queries.GetMotors;
public record GetMotorsQuery(QueryParams Params, Guid? LocationId, Guid? CenterId, double? MinRunningHours) : IRequest<PaginatedResult<MotorDto>>;
