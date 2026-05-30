using MediatR;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;

namespace WLR.Application.Features.Motors.Queries.GetMotorHistory;

public record GetMotorHistoryQuery(
    int PageNumber,
    int PageSize,
    string? DateFilter,  // today | yesterday | 7days | custom
    DateTime? StartDate,
    DateTime? EndDate,
    Guid? MotorId,
    Guid? CenterId,
    Guid? LocationId,
    string? MotorSearch,
    double? MinDurationHours
) : IRequest<PaginatedResult<MotorSessionDto>>;
