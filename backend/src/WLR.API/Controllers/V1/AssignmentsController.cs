using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
using WLR.Application.Features.Assignments.Commands.CreateAssignment;
using WLR.Application.Features.Assignments.Commands.DeleteAssignment;
using WLR.Application.Features.Assignments.Commands.RevokeAssignment;
using WLR.Application.Features.Assignments.Queries.GetAssignmentById;
using WLR.Application.Features.Assignments.Queries.GetAssignments;

namespace WLR.API.Controllers.V1;

[Authorize]
public class AssignmentsController : BaseController
{
    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<AssignmentDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] QueryParams query, [FromQuery] Guid? centerId, [FromQuery] Guid? userId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAssignmentsQuery(query, centerId, userId), cancellationToken);
        return Ok(ApiResponse<PaginatedResult<AssignmentDto>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentDto>), 200)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAssignmentByIdQuery(id), cancellationToken);
        return Ok(ApiResponse<AssignmentDto>.Ok(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<AssignmentDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateAssignmentRequest request, CancellationToken cancellationToken)
    {
        var cmd = new CreateAssignmentCommand(request.UserId, request.CenterId, request.LocationId, request.MotorId, request.Notes);
        var result = await Mediator.Send(cmd, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<AssignmentDto>.Ok(result, "Assignment created successfully."));
    }

    [HttpPut("{id:guid}/revoke")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Revoke(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new RevokeAssignmentCommand(id), cancellationToken);
        return Ok(ApiResponse.Ok("Assignment revoked successfully."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse), 200)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteAssignmentCommand(id), cancellationToken);
        return Ok(ApiResponse.Ok("Assignment deleted successfully."));
    }
}
