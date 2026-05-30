using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
using WLR.Application.Features.Locations.Commands.CreateLocation;
using WLR.Application.Features.Locations.Commands.DeleteLocation;
using WLR.Application.Features.Locations.Commands.UpdateLocation;
using WLR.Application.Features.Locations.Queries.GetLocationById;
using WLR.Application.Features.Locations.Queries.GetLocations;

namespace WLR.API.Controllers.V1;

[Authorize]
public class LocationsController : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<LocationDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] QueryParams query, [FromQuery] Guid? centerId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetLocationsQuery(query, centerId), cancellationToken);
        return Ok(ApiResponse<PaginatedResult<LocationDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<LocationDto>), 200)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetLocationByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<LocationDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<LocationDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateLocationRequest request, CancellationToken cancellationToken)
    {
        var cmd = new CreateLocationCommand(request.CenterId, request.Name, request.Description, request.Floor, request.Zone);
        var result = await Mediator.Send(cmd, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<LocationDto>.Ok(result, "Location created successfully."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<LocationDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLocationRequest request, CancellationToken cancellationToken)
    {
        var cmd = new UpdateLocationCommand(id, request.Name, request.Description, request.Floor, request.Zone);
        var result = await Mediator.Send(cmd, cancellationToken);
        return Ok(ApiResponse<LocationDto>.Ok(result, "Location updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteLocationCommand(id), cancellationToken);
        return Ok(ApiResponse.Ok("Location deleted successfully."));
    }
}
