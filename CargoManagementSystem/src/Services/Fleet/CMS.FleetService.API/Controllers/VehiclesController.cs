using System.Security.Claims;
using CMS.FleetService.Application.Commands.AddMaintenanceLog;
using CMS.FleetService.Application.Commands.AddVehicle;
using CMS.FleetService.Application.Commands.SubmitGpsUpdate;
using CMS.FleetService.Application.Commands.UpdateVehicle;
using CMS.FleetService.Application.Commands.UpdateVehicleStatus;
using CMS.FleetService.Application.Commands.CreateAssignment;
using CMS.FleetService.Domain.Interfaces;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Application.Queries.GetAvailableVehicles;
using CMS.FleetService.Application.Queries.GetLiveLocations;
using CMS.FleetService.Application.Queries.GetMaintenanceLogs;
using CMS.FleetService.Application.Queries.GetVehicle;
using CMS.FleetService.Application.Queries.GetVehicleRoute;
using CMS.FleetService.Application.Queries.GetVehicles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.FleetService.API.Controllers;

[ApiController]
[Route("api/v1/vehicles")]
public class VehiclesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VehiclesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Register a new vehicle.</summary>
    [HttpPost]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> AddVehicle([FromBody] AddVehicleRequest request)
    {
        var result = await _mediator.Send(new AddVehicleCommand(request));
        return result.Success
            ? CreatedAtAction(nameof(GetVehicle), new { vehicleId = result.Data!.Id }, result)
            : BadRequest(result);
    }

    /// <summary>Get paginated list of vehicles.</summary>
    [HttpGet]
    [Authorize(Roles = "OpsManager,Dispatcher,SuperAdmin")]
    public async Task<IActionResult> GetVehicles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? type = null)
    {
        var result = await _mediator.Send(new GetVehiclesQuery(page, pageSize, status, type));
        return Ok(result);
    }

    /// <summary>Get vehicle by ID.</summary>
    [HttpGet("{vehicleId:guid}")]
    [Authorize(Roles = "OpsManager,Dispatcher,SuperAdmin")]
    public async Task<IActionResult> GetVehicle(Guid vehicleId)
    {
        var result = await _mediator.Send(new GetVehicleQuery(vehicleId));
        return Ok(result);
    }

    /// <summary>Get available vehicles for dispatch board.</summary>
    [HttpGet("available")]
    [Authorize(Roles = "OpsManager,Dispatcher,SuperAdmin")]
    public async Task<IActionResult> GetAvailableVehicles(
        [FromQuery] decimal? minCapacityKg = null,
        [FromQuery] string? type = null)
    {
        var result = await _mediator.Send(new GetAvailableVehiclesQuery(minCapacityKg, type));
        return Ok(result);
    }

    /// <summary>Get live GPS locations of all active vehicles.</summary>
    [HttpGet("live-locations")]
    [Authorize(Roles = "OpsManager,Dispatcher,SuperAdmin")]
    public async Task<IActionResult> GetLiveLocations()
    {
        var result = await _mediator.Send(new GetLiveLocationsQuery());
        return Ok(result);
    }

    /// <summary>Update vehicle details.</summary>
    [HttpPatch("{vehicleId:guid}")]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> UpdateVehicle(Guid vehicleId, [FromBody] UpdateVehicleRequest request)
    {
        var result = await _mediator.Send(new UpdateVehicleCommand(vehicleId, request));
        return Ok(result);
    }

    /// <summary>Set vehicle status (Inactive, Maintenance, Available).</summary>
    [HttpPatch("{vehicleId:guid}/status")]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> UpdateVehicleStatus(Guid vehicleId, [FromBody] UpdateVehicleStatusRequest request)
    {
        var result = await _mediator.Send(new UpdateVehicleStatusCommand(vehicleId, request));
        return Ok(result);
    }

    /// <summary>Log a maintenance entry for a vehicle.</summary>
    [HttpPost("{vehicleId:guid}/maintenance")]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> AddMaintenanceLog(Guid vehicleId, [FromBody] AddMaintenanceLogRequest request)
    {
        var result = await _mediator.Send(new AddMaintenanceLogCommand(vehicleId, request));
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>Get GPS breadcrumb route for a vehicle.</summary>
    [HttpGet("{vehicleId:guid}/route")]
    [Authorize(Roles = "OpsManager,Dispatcher,SuperAdmin")]
    public async Task<IActionResult> GetVehicleRoute(Guid vehicleId, [FromQuery] Guid? assignmentId = null)
    {
        var result = await _mediator.Send(new GetVehicleRouteQuery(vehicleId, assignmentId));
        return Ok(result);
    }

    /// <summary>Submit GPS update from driver.</summary>
    [HttpPost("{vehicleId:guid}/gps")]
    [Authorize(Roles = "Driver,SuperAdmin")]
    public async Task<IActionResult> SubmitGpsUpdate(Guid vehicleId, [FromBody] SubmitGpsUpdateRequest request)
    {
        var driverIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(driverIdClaim, out var driverId))
            return Unauthorized();

        var result = await _mediator.Send(new SubmitGpsUpdateCommand(vehicleId, driverId, request));
        return Ok(result);
    }

    /// <summary>Get maintenance history for a vehicle (GAP-005).</summary>
    [HttpGet("{vehicleId:guid}/maintenance")]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetMaintenanceLogs(
        Guid vehicleId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetMaintenanceLogsQuery(vehicleId, page, pageSize));
        return Ok(result);
    }

    /// <summary>Assign or reassign a driver to a vehicle.</summary>
    [HttpPatch("{vehicleId:guid}/driver")]
    [Authorize(Roles = "Dispatcher,OpsManager,SuperAdmin")]
    public async Task<IActionResult> AssignDriver(
        Guid vehicleId,
        [FromBody] AssignDriverToVehicleRequest request,
        [FromServices] IDriverRepository driverRepository,
        [FromServices] IVehicleRepository vehicleRepository,
        [FromServices] IAssignmentRepository assignmentRepository)
    {
        try
        {
            var vehicle = await vehicleRepository.GetByIdAsync(vehicleId);
            if (vehicle == null)
                return NotFound(new { success = false, message = "Vehicle not found" });

            var driver = await driverRepository.GetByIdAsync(request.DriverId);
            if (driver == null)
                return NotFound(new { success = false, message = "Driver not found" });

            // Create the assignment record
            var assignment = CMS.FleetService.Domain.Entities.Assignment.Create(
                Guid.Empty, // No shipment — direct driver-vehicle link
                request.DriverId,
                vehicleId,
                null,
                "Driver assigned to vehicle via fleet management");

            // Update statuses
            driver.SetStatus(CMS.FleetService.Domain.Enums.DriverStatus.OnDuty);
            vehicle.SetStatus(CMS.FleetService.Domain.Enums.VehicleStatus.InUse);

            await assignmentRepository.AddAsync(assignment);
            await driverRepository.UpdateAsync(driver);
            await vehicleRepository.UpdateAsync(vehicle);

            return Ok(new
            {
                success = true,
                message = "Driver assigned to vehicle successfully",
                data = new
                {
                    assignmentId = assignment.Id,
                    vehicleId,
                    driverId = request.DriverId,
                    status = "Active"
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = $"Failed to assign driver: {ex.Message}" });
        }
    }
}

public class AssignDriverToVehicleRequest
{
    public Guid DriverId { get; set; }
}

