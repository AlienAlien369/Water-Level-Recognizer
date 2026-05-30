using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WLR.Application.Common.Models;

namespace WLR.API.Controllers.V1;

[Authorize]
public class NotificationsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetMyNotifications([FromQuery] QueryParams query, CancellationToken cancellationToken)
        => Ok(ApiResponse<object>.Ok(new { message = "Notifications endpoint ready" }));

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken cancellationToken)
        => Ok(ApiResponse.Ok("Notification marked as read."));

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken)
        => Ok(ApiResponse.Ok("All notifications marked as read."));
}
