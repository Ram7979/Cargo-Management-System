using CMS.FleetService.Application.Commands.CreateAssignment;
using CMS.FleetService.Application.Commands.UnassignAssignment;
using CMS.FleetService.Application.Commands.UpdateAssignment;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Application.Queries.GetAssignment;
using CMS.FleetService.Application.Queries.GetAssignments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.FleetService.API.Controllers;

[ApiController]
[Route("api/v1/assignments")]
public class AssignmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AssignmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Create a new assignment (assign shipment to driver and vehicle).</summary>
    [HttpPost]
    [Authorize(Roles = "Dispatcher,OpsManager,SuperAdmin")]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentRequest request)
    {
        var result = await _mediator.Send(new CreateAssignmentCommand(request));
        return result.Success
            ? CreatedAtAction(nameof(GetAssignment), new { assignmentId = result.Data!.Id }, result)
            : BadRequest(result);
    }

    /// <summary>Get paginated assignment list for Dispatch Board.</summary>
    [HttpGet]
    [Authorize(Roles = "Dispatcher,OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetAssignments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] Guid? driverId = null,
        [FromQuery] Guid? vehicleId = null,
        [FromQuery] Guid? shipmentId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        var result = await _mediator.Send(new GetAssignmentsQuery(
            page, pageSize, status, driverId, vehicleId, shipmentId, dateFrom, dateTo));
        return Ok(result);
    }

    /// <summary>Get assignment by ID.</summary>
    [HttpGet("{assignmentId:guid}")]
    [Authorize(Roles = "Dispatcher,OpsManager,Driver,SuperAdmin")]
    public async Task<IActionResult> GetAssignment(Guid assignmentId)
    {
        var result = await _mediator.Send(new GetAssignmentQuery(assignmentId));
        return Ok(result);
    }

    /// <summary>Update assignment (reschedule, swap vehicle, update notes).</summary>
    [HttpPatch("{assignmentId:guid}")]
    [Authorize(Roles = "Dispatcher,OpsManager,SuperAdmin")]
    public async Task<IActionResult> UpdateAssignment(Guid assignmentId, [FromBody] UpdateAssignmentRequest request)
    {
        var result = await _mediator.Send(new UpdateAssignmentCommand(assignmentId, request));
        return Ok(result);
    }

    /// <summary>Unassign / cancel an active assignment. Releases driver and vehicle.</summary>
    [HttpDelete("{assignmentId:guid}")]
    [Authorize(Roles = "Dispatcher,OpsManager,SuperAdmin")]
    public async Task<IActionResult> UnassignAssignment(Guid assignmentId, [FromBody] UnassignRequest request)
    {
        var result = await _mediator.Send(new UnassignAssignmentCommand(assignmentId, request));
        return Ok(result);
    }
}
