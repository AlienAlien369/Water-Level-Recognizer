using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WLR.Application.Common.Models;

namespace WLR.API.Controllers.V1;

[Authorize(Roles = "Admin,SuperAdmin")]
public class AuditLogsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParams query, [FromQuery] string? entityType, [FromQuery] Guid? userId, CancellationToken cancellationToken)
        => Ok(ApiResponse<object>.Ok(new { message = "Audit logs endpoint ready" }));
}
