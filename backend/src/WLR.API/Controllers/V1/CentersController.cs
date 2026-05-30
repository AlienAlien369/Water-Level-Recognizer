using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
using WLR.Application.Features.Centers.Commands.CreateCenter;
using WLR.Application.Features.Centers.Commands.DeleteCenter;
using WLR.Application.Features.Centers.Commands.ToggleCenterAssignment;
using WLR.Application.Features.Centers.Commands.UpdateCenter;
using WLR.Application.Features.Centers.Queries.GetCenterById;
using WLR.Application.Features.Centers.Queries.GetCenters;

namespace WLR.API.Controllers.V1;

[Authorize]
public class CentersController : BaseController
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<CenterDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] QueryParams query, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCentersQuery(query), cancellationToken);
        return Ok(ApiResponse<PaginatedResult<CenterDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CenterDto>), 200)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCenterByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<CenterDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<CenterDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateCenterRequest request, CancellationToken cancellationToken)
    {
        var cmd = new CreateCenterCommand(request.Name, request.Description, request.Address, request.City, request.State, request.Country, request.ContactPhone, request.ContactEmail);
        var result = await Mediator.Send(cmd, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<CenterDto>.Ok(result, "Center created successfully."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<CenterDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCenterRequest request, CancellationToken cancellationToken)
    {
        var cmd = new UpdateCenterCommand(id, request.Name, request.Description, request.Address, request.City, request.State, request.Country, request.ContactPhone, request.ContactEmail);
        var result = await Mediator.Send(cmd, cancellationToken);
        return Ok(ApiResponse<CenterDto>.Ok(result, "Center updated successfully."));
    }

    [HttpPatch("{id:guid}/assignment-mode")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<CenterDto>), 200)]
    public async Task<IActionResult> ToggleAssignmentMode(Guid id, [FromBody] ToggleCenterAssignmentRequest request, CancellationToken cancellationToken)
    {
        var cmd = new ToggleCenterAssignmentCommand(id, request.RequiresAssignment);
        var result = await Mediator.Send(cmd, cancellationToken);
        return Ok(ApiResponse<CenterDto>.Ok(result, $"Assignment mode set to {(request.RequiresAssignment ? "required" : "open access")}."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteCenterCommand(id), cancellationToken);
        return Ok(ApiResponse.Ok("Center deleted successfully."));
    }
}
