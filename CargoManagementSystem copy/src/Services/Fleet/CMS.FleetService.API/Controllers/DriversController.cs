using System.Security.Claims;
using CMS.FleetService.Application.Commands.RegisterDriver;
using CMS.FleetService.Application.Commands.UpdateDriverStatus;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Application.Queries.GetAvailableDrivers;
using CMS.FleetService.Application.Queries.GetDriver;
using CMS.FleetService.Application.Queries.GetDriverActiveAssignment;
using CMS.FleetService.Application.Queries.GetDriverPerformance;
using CMS.FleetService.Application.Queries.GetDrivers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.FleetService.API.Controllers;

[ApiController]
[Route("api/v1/drivers")]
public class DriversController : ControllerBase
{
    private readonly IMediator _mediator;

    public DriversController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Register a new driver.</summary>
    [HttpPost]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> RegisterDriver([FromBody] RegisterDriverRequest request)
    {
        var result = await _mediator.Send(new RegisterDriverCommand(request));
        return result.Success
            ? CreatedAtAction(nameof(GetDriver), new { driverId = result.Data!.Id }, result)
            : BadRequest(result);
    }

    /// <summary>Get paginated list of drivers.</summary>
    [HttpGet]
    [Authorize(Roles = "OpsManager,Dispatcher,SuperAdmin")]
    public async Task<IActionResult> GetDrivers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        var result = await _mediator.Send(new GetDriversQuery(page, pageSize, status));
        return Ok(result);
    }

    /// <summary>Get driver profile by ID.</summary>
    [HttpGet("{driverId:guid}")]
    [Authorize(Roles = "OpsManager,Dispatcher,Driver,SuperAdmin")]
    public async Task<IActionResult> GetDriver(Guid driverId)
    {
        var result = await _mediator.Send(new GetDriverQuery(driverId));
        return Ok(result);
    }

    /// <summary>Update driver status (Available / OffDuty).</summary>
    [HttpPatch("{driverId:guid}/status")]
    [Authorize(Roles = "OpsManager,Driver,SuperAdmin")]
    public async Task<IActionResult> UpdateDriverStatus(Guid driverId, [FromBody] UpdateDriverStatusRequest request)
    {
        var result = await _mediator.Send(new UpdateDriverStatusCommand(driverId, request));
        return Ok(result);
    }

    /// <summary>Get available drivers for dispatch board.</summary>
    [HttpGet("available")]
    [Authorize(Roles = "Dispatcher,OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetAvailableDrivers()
    {
        var result = await _mediator.Send(new GetAvailableDriversQuery());
        return Ok(result);
    }

    /// <summary>Get active assignment for a driver.</summary>
    [HttpGet("{driverId:guid}/assignment")]
    [Authorize(Roles = "Dispatcher,OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetDriverAssignment(Guid driverId)
    {
        var result = await _mediator.Send(new GetDriverActiveAssignmentQuery(driverId));
        return Ok(result);
    }

    /// <summary>Get driver performance report.</summary>
    [HttpGet("{driverId:guid}/performance")]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetDriverPerformance(
        Guid driverId,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        var result = await _mediator.Send(new GetDriverPerformanceQuery(driverId, dateFrom, dateTo));
        return Ok(result);
    }
}
