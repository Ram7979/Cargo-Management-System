using System.Security.Claims;
using CMS.ShipmentService.Application.Commands.CancelShipment;
using CMS.ShipmentService.Application.Commands.CreateShipment;
using CMS.ShipmentService.Application.Commands.UpdateLocation;
using CMS.ShipmentService.Application.Commands.UpdateShipmentStatus;
using CMS.ShipmentService.Application.DTOs;
using CMS.ShipmentService.Application.Queries.CalculateRate;
using CMS.ShipmentService.Application.Queries.GetShipment;
using CMS.ShipmentService.Application.Queries.GetShipments;
using CMS.ShipmentService.Application.Queries.TrackShipment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.ShipmentService.API.Controllers;

[ApiController]
[Route("api/v1/shipments")]
public class ShipmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShipmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Create a new shipment booking.</summary>
    [HttpPost]
    [Authorize(Roles = "OpsManager,Dispatcher,SuperAdmin")]
    public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentRequest request)
    {
        var result = await _mediator.Send(new CreateShipmentCommand(request));
        return result.Success
            ? CreatedAtAction(nameof(GetShipment), new { shipmentId = result.Data!.Id }, result)
            : BadRequest(result);
    }

    /// <summary>Get shipment by ID.</summary>
    [HttpGet("{shipmentId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetShipment(Guid shipmentId)
    {
        var result = await _mediator.Send(new GetShipmentQuery(shipmentId));
        return Ok(result);
    }

    /// <summary>Track shipment by tracking number (public).</summary>
    [HttpGet("track/{trackingNumber}")]
    [AllowAnonymous]
    public async Task<IActionResult> TrackShipment(string trackingNumber)
    {
        var result = await _mediator.Send(new TrackShipmentQuery(trackingNumber));
        return Ok(result);
    }

    /// <summary>Update shipment status. Includes GPS, failure reason, re-delivery scheduling.</summary>
    [HttpPatch("{shipmentId:guid}/status")]
    [Authorize(Roles = "OpsManager,Driver,Dispatcher,SuperAdmin")]
    public async Task<IActionResult> UpdateStatus(Guid shipmentId, [FromBody] UpdateShipmentStatusRequest request)
    {
        var result = await _mediator.Send(new UpdateShipmentStatusCommand(shipmentId, request));
        return Ok(result);
    }

    /// <summary>Cancel a shipment (Pending or Assigned only).</summary>
    [HttpDelete("{shipmentId:guid}")]
    [Authorize(Roles = "OpsManager,Customer,SuperAdmin")]
    public async Task<IActionResult> CancelShipment(Guid shipmentId, [FromBody] CancelShipmentRequest request)
    {
        var actorId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        var result = await _mediator.Send(new CancelShipmentCommand(shipmentId, request, actorId));
        return Ok(result);
    }

    /// <summary>Update GPS location for a shipment (Driver only).</summary>
    [HttpPatch("{shipmentId:guid}/location")]
    [Authorize(Roles = "Driver")]
    public async Task<IActionResult> UpdateLocation(Guid shipmentId, [FromBody] UpdateLocationRequest request)
    {
        var result = await _mediator.Send(new UpdateLocationCommand(shipmentId, request));
        return Ok(result);
    }

    /// <summary>List shipments with extended filters.</summary>
    [HttpGet]
    [Authorize(Roles = "OpsManager,Support,FinanceOfficer,SuperAdmin")]
    public async Task<IActionResult> GetShipments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] Guid? customerId = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] string? serviceType = null,
        [FromQuery] string? trackingNumber = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDir = "desc")
    {
        var result = await _mediator.Send(new GetShipmentsQuery(
            page, pageSize, status, customerId, dateFrom, dateTo, serviceType, trackingNumber, sortBy, sortDir));
        return Ok(result);
    }

    /// <summary>Get BOL document URL for a shipment.</summary>
    [HttpGet("{shipmentId:guid}/document/bol")]
    [Authorize(Roles = "OpsManager,Customer,Dispatcher,SuperAdmin")]
    public async Task<IActionResult> GetBolDocument(Guid shipmentId)
    {
        var result = await _mediator.Send(new GetShipmentQuery(shipmentId));
        if (!result.Success || result.Data == null) return NotFound(result);
        if (string.IsNullOrWhiteSpace(result.Data.BolDocumentUrl))
            return NotFound(new { message = "BOL document not available." });
        return Ok(new { bolUrl = result.Data.BolDocumentUrl });
    }

    /// <summary>Get POD document for a delivered shipment.</summary>
    [HttpGet("{shipmentId:guid}/document/pod")]
    [Authorize(Roles = "OpsManager,Customer,Dispatcher,SuperAdmin")]
    public async Task<IActionResult> GetPodDocument(Guid shipmentId)
    {
        var result = await _mediator.Send(new GetShipmentQuery(shipmentId));
        if (!result.Success || result.Data == null) return NotFound(result);
        if (result.Data.Status != "Delivered")
            return BadRequest(new { message = "POD is only available for delivered shipments." });
        if (string.IsNullOrWhiteSpace(result.Data.PodImageUrl))
            return NotFound(new { message = "POD document not available." });
        return Ok(new { podImageUrl = result.Data.PodImageUrl, podSignatureData = result.Data.PodImageUrl });
    }
}
