using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
using WLR.Application.Features.Users.Commands.DemoteUser;
using WLR.Application.Features.Users.Commands.PromoteUser;
using WLR.Application.Features.Users.Commands.ToggleUserActive;
using WLR.Application.Features.Users.Commands.UpdateUserProfile;
using WLR.Application.Features.Users.Queries.GetUserById;
using WLR.Application.Features.Users.Queries.GetUsers;

namespace WLR.API.Controllers.V1;

[Authorize]
public class UsersController : BaseController
{
    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<UserDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] QueryParams query, [FromQuery] Guid? centerId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetUsersQuery(query, centerId), cancellationToken);
        return Ok(ApiResponse<PaginatedResult<UserDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<UserDto>.Ok(result));
    }

    [HttpPut("{id:guid}/profile")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        var cmd = new UpdateUserProfileCommand(id, request.Name, request.Email, request.ProfileImageUrl);
        var result = await Mediator.Send(cmd, cancellationToken);
        return Ok(ApiResponse<UserDto>.Ok(result, "Profile updated successfully."));
    }

    [HttpPut("{id:guid}/promote")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    public async Task<IActionResult> PromoteToAdmin(Guid id, [FromQuery] Guid centerId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new PromoteUserCommand(id, centerId), cancellationToken);
        return Ok(ApiResponse<UserDto>.Ok(result, "User promoted to Admin."));
    }

    [HttpPut("{id:guid}/demote")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
    public async Task<IActionResult> DemoteToUser(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DemoteUserCommand(id), cancellationToken);
        return Ok(ApiResponse<UserDto>.Ok(result, "User demoted to Sewadar."));
    }

    [HttpPut("{id:guid}/activate")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new ToggleUserActiveCommand(id, true), cancellationToken);
        return Ok(ApiResponse.Ok("User activated."));
    }

    [HttpPut("{id:guid}/deactivate")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new ToggleUserActiveCommand(id, false), cancellationToken);
        return Ok(ApiResponse.Ok("User deactivated."));
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        => Ok(ApiResponse<object>.Ok(new { message = "Profile endpoint ready" }));
}
