using MediatR;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Dashboard.Queries.GetDashboardSummary;
public record GetDashboardSummaryQuery(Guid? CenterId = null) : IRequest<DashboardSummaryDto>;
