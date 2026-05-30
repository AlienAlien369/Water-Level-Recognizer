using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
using WLR.Application.Features.Motors.Commands.CloseMotor;
using WLR.Application.Features.Motors.Commands.CreateMotor;
using WLR.Application.Features.Motors.Commands.DeleteMotor;
using WLR.Application.Features.Motors.Commands.OpenMotor;
using WLR.Application.Features.Motors.Commands.UpdateMotor;
using WLR.Application.Features.Motors.Queries.GetMotorById;
using WLR.Application.Features.Motors.Queries.GetMotors;
using WLR.Application.Features.Motors.Queries.GetMotorHistory;

namespace WLR.API.Controllers.V1;

[Authorize]
public class MotorsController : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<MotorDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] QueryParams query, [FromQuery] Guid? locationId, [FromQuery] Guid? centerId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetMotorsQuery(query, locationId, centerId), cancellationToken);
        return Ok(ApiResponse<PaginatedResult<MotorDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MotorDto>), 200)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetMotorByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<MotorDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<MotorDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateMotorRequest request, CancellationToken cancellationToken)
    {
        var cmd = new CreateMotorCommand(request.LocationId, request.MotorNumber, request.Description, request.WaterCapacityLiters);
        var result = await Mediator.Send(cmd, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<MotorDto>.Ok(result, "Motor created successfully."));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<MotorDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMotorRequest request, CancellationToken cancellationToken)
    {
        var cmd = new UpdateMotorCommand(id, request.MotorNumber, request.Description, request.WaterCapacityLiters);
        var result = await Mediator.Send(cmd, cancellationToken);
        return Ok(ApiResponse<MotorDto>.Ok(result, "Motor updated successfully."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteMotorCommand(id), cancellationToken);
        return Ok(ApiResponse.Ok("Motor deleted successfully."));
    }

    [HttpPost("{id:guid}/open")]
    public async Task<IActionResult> Open(Guid id, [FromBody] OpenMotorRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new OpenMotorCommand(id, request.Notes), cancellationToken);
        return Ok(ApiResponse<MotorLogDto>.Ok(result, "Motor opened successfully."));
    }

    [HttpPost("{id:guid}/close")]
    public async Task<IActionResult> Close(Guid id, [FromBody] CloseMotorRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new CloseMotorCommand(id, request.Notes), cancellationToken);
        return Ok(ApiResponse<MotorLogDto>.Ok(result, "Motor closed successfully."));
    }

    [HttpGet("{id:guid}/logs")]
    public async Task<IActionResult> GetLogs(Guid id, [FromQuery] QueryParams query, CancellationToken cancellationToken)
    {
        return Ok(ApiResponse<object>.Ok(new { motorId = id, message = "Motor logs endpoint ready" }));
    }

    [HttpGet("history")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<MotorSessionDto>>), 200)]
    public async Task<IActionResult> GetHistory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? dateFilter = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] Guid? motorId = null,
        [FromQuery] Guid? centerId = null,
        CancellationToken cancellationToken = default)
    {
        var queryCmd = new GetMotorHistoryQuery(pageNumber, pageSize, dateFilter, startDate, endDate, motorId, centerId);
        var result = await Mediator.Send(queryCmd, cancellationToken);
        return Ok(ApiResponse<PaginatedResult<MotorSessionDto>>.Ok(result));
    }
}
