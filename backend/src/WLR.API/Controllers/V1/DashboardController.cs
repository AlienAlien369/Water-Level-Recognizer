using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
using WLR.Application.Features.Dashboard.Queries.GetDashboardSummary;

namespace WLR.API.Controllers.V1;

[Authorize]
public class DashboardController : BaseController
{
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<DashboardSummaryDto>), 200)]
    public async Task<IActionResult> GetSummary([FromQuery] Guid? centerId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetDashboardSummaryQuery(centerId), cancellationToken);
        return Ok(ApiResponse<DashboardSummaryDto>.Ok(result));
    }
}
