using System.Security.Claims;
using CMS.ShipmentService.Application.Commands.CancelShipment;
using CMS.ShipmentService.Application.Commands.CreateShipment;
using CMS.ShipmentService.Application.Commands.UpdateLocation;
using CMS.ShipmentService.Application.Commands.UpdateShipmentStatus;
using CMS.ShipmentService.Application.DTOs;
using CMS.ShipmentService.Application.Queries.CalculateRate;
using CMS.ShipmentService.Application.Queries.GetShipment;
using CMS.ShipmentService.Application.Queries.GetShipmentStatusHistory;
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
    [Authorize(Roles = "OpsManager,Dispatcher,SuperAdmin,Customer")]
    public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentRequest request)
    {
        // If customer, force their own ID
        if (User.IsInRole("Customer"))
        {
            request.CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        var result = await _mediator.Send(new CreateShipmentCommand(request));
        return result.Success
            ? CreatedAtAction(nameof(GetShipment), new { shipmentId = result.Data!.Id }, result)
            : BadRequest(result);
    }

    /// <summary>Get shipments for the calling user.</summary>
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetMyShipments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _mediator.Send(new GetShipmentsQuery(
            page, pageSize, status, userId, null, null, null, null, null, "desc"));
        return Ok(result);
    }

    /// <summary>Get shipment by ID.</summary>
    [HttpGet("{shipmentId:guid}")]
    [Authorize(Roles = "OpsManager,Dispatcher,Driver,Customer,Support,FinanceOfficer,WarehouseManager,SuperAdmin")]
    public async Task<IActionResult> GetShipment(Guid shipmentId)
    {
        var result = await _mediator.Send(new GetShipmentQuery(shipmentId));

        // Customer ownership check
        if (result.Success && User.IsInRole("Customer"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (result.Data?.CustomerId != userId)
                return Forbid();
        }

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
    [HttpPut("{shipmentId:guid}/status")]
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
        
        // Ownership check for customer
        if (User.IsInRole("Customer"))
        {
            var shipmentRes = await _mediator.Send(new GetShipmentQuery(shipmentId));
            if (shipmentRes.Data?.CustomerId != actorId) return Forbid();
        }

        var result = await _mediator.Send(new CancelShipmentCommand(shipmentId, request, actorId));
        return Ok(result);
    }

    /// <summary>Update GPS location for a shipment (Driver only).</summary>
    [HttpPatch("{shipmentId:guid}/location")]
    [Authorize(Roles = "Driver,SuperAdmin")]
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
        [FromQuery] string? customerId = null,
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

    /// <summary>Get BOL document for a shipment (Generates PDF).</summary>
    [HttpGet("{shipmentId:guid}/document/bol")]
    [Authorize(Roles = "OpsManager,Customer,Dispatcher,SuperAdmin")]
    public async Task<IActionResult> GetBolDocument(Guid shipmentId)
    {
        var result = await _mediator.Send(new GetShipmentQuery(shipmentId));
        if (!result.Success || result.Data == null) return NotFound(result);

        var shipment = result.Data;

        // Mock PDF Generation
        // In a real app, use QuestPDF, iTextSharp or similar
        using (var ms = new MemoryStream())
        {
            using (var writer = new StreamWriter(ms))
            {
                writer.WriteLine("%PDF-1.4");
                writer.WriteLine("1 0 obj <</Type /Catalog /Pages 2 0 R>> endobj");
                writer.WriteLine("2 0 obj <</Type /Pages /Kids [3 0 R] /Count 1>> endobj");
                writer.WriteLine("3 0 obj <</Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources <<>>>> endobj");
                
                var content = $"BT /F1 12 Tf 100 700 Td (BILL OF LADING) Tj " +
                              $"0 -20 Td (Shipment ID: {shipment.Id}) Tj " +
                              $"0 -20 Td (Tracking: {shipment.TrackingNumber}) Tj " +
                              $"0 -20 Td (Sender: {shipment.SenderName}) Tj " +
                              $"0 -20 Td (Recipient: {shipment.RecipientName}) Tj " +
                              $"0 -20 Td (Cargo: {shipment.CargoType} - {shipment.WeightKg}KG) Tj " +
                              $"0 -20 Td (Status: {shipment.Status}) Tj " +
                              $"0 -20 Td (Date: {DateTime.UtcNow:yyyy-MM-dd}) Tj ET";
                              
                writer.WriteLine($"4 0 obj <</Length {content.Length}>> stream");
                writer.WriteLine(content);
                writer.WriteLine("endstream endobj");
                writer.WriteLine("xref");
                writer.WriteLine("0 5");
                writer.WriteLine("0000000000 65535 f");
                writer.WriteLine("0000000010 00000 n");
                writer.WriteLine("0000000060 00000 n");
                writer.WriteLine("0000000115 00000 n");
                writer.WriteLine("0000000210 00000 n");
                writer.WriteLine("trailer <</Size 5 /Root 1 0 R>>");
                writer.WriteLine("startxref");
                writer.WriteLine("300");
                writer.WriteLine("%%EOF");
                writer.Flush();
            }
            
            return File(ms.ToArray(), "application/pdf", $"BOL_{shipment.TrackingNumber}.pdf");
        }
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

    /// <summary>Get full status history timeline for a shipment (GAP-004).</summary>
    [HttpGet("{shipmentId:guid}/status-history")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStatusHistory(Guid shipmentId)
    {
        var result = await _mediator.Send(new GetShipmentStatusHistoryQuery(shipmentId));
        return Ok(result);
    }
}
