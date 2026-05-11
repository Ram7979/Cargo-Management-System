using System.Security.Claims;
using CMS.WarehouseService.Application.Commands.AddBin;
using CMS.WarehouseService.Application.Commands.CreateWarehouse;
using CMS.WarehouseService.Application.Commands.ReceiveCargo;
using CMS.WarehouseService.Application.Commands.ReleaseCargo;
using CMS.WarehouseService.Application.Commands.UpdateBin;
using CMS.WarehouseService.Application.Commands.UpdateDamageReport;
using CMS.WarehouseService.Application.Commands.UpdateWarehouse;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Application.Queries.GetBins;
using CMS.WarehouseService.Application.Queries.GetDamageReports;
using CMS.WarehouseService.Application.Queries.GetInventory;
using CMS.WarehouseService.Application.Queries.GetReceipt;
using CMS.WarehouseService.Application.Queries.GetReceipts;
using CMS.WarehouseService.Application.Queries.GetWarehouse;
using CMS.WarehouseService.Application.Queries.GetWarehouses;
using CMS.WarehouseService.Application.Queries.LookupShipment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.WarehouseService.API.Controllers;

[ApiController]
[Route("api/v1/warehouse")]
public class WarehouseController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehouseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string ActorUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    // ─────────────────────────────────────────────────────────────────────────
    // Warehouse CRUD
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Register a new warehouse.</summary>
    [HttpPost]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseRequest request)
    {
        var result = await _mediator.Send(new CreateWarehouseCommand(request));
        return result.Success
            ? CreatedAtAction(nameof(GetWarehouse), new { warehouseId = result.Data!.Id }, result)
            : BadRequest(result);
    }

    /// <summary>List all warehouses with optional filters.</summary>
    [HttpGet]
    [Authorize(Roles = "OpsManager,WarehouseManager,Dispatcher,SuperAdmin")]
    public async Task<IActionResult> GetWarehouses(
        [FromQuery] string? city = null,
        [FromQuery] string? country = null,
        [FromQuery] bool? hasAvailableBins = null)
    {
        var result = await _mediator.Send(new GetWarehousesQuery(city, country, hasAvailableBins));
        return Ok(result);
    }

    /// <summary>Get a single warehouse by ID with full bin layout.</summary>
    [HttpGet("{warehouseId:guid}")]
    [Authorize(Roles = "OpsManager,WarehouseManager,WarehouseOperator,SuperAdmin")]
    public async Task<IActionResult> GetWarehouse(Guid warehouseId)
    {
        var result = await _mediator.Send(new GetWarehouseQuery(warehouseId));
        return Ok(result);
    }

    /// <summary>Update warehouse details.</summary>
    [HttpPatch("{warehouseId:guid}")]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> UpdateWarehouse(Guid warehouseId, [FromBody] UpdateWarehouseRequest request)
    {
        var result = await _mediator.Send(new UpdateWarehouseCommand(warehouseId, request));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Get inventory summary for a warehouse.</summary>
    [HttpGet("{warehouseId:guid}/inventory")]
    [Authorize(Roles = "OpsManager,WarehouseManager,SuperAdmin")]
    public async Task<IActionResult> GetInventory(Guid warehouseId)
    {
        var result = await _mediator.Send(new GetInventoryQuery(warehouseId));
        return Ok(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Bin Management
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>List bins for a warehouse with optional filters.</summary>
    [HttpGet("{warehouseId:guid}/bins")]
    [Authorize(Roles = "OpsManager,WarehouseManager,WarehouseOperator,SuperAdmin")]
    public async Task<IActionResult> GetBins(
        Guid warehouseId,
        [FromQuery] bool? available = null,
        [FromQuery] string? zone = null,
        [FromQuery] string? level = null)
    {
        var result = await _mediator.Send(new GetBinsQuery(warehouseId, available, zone, level));
        return Ok(result);
    }

    /// <summary>Add a new bin to a warehouse.</summary>
    [HttpPost("{warehouseId:guid}/bins")]
    [Authorize(Roles = "OpsManager,WarehouseManager,SuperAdmin")]
    public async Task<IActionResult> AddBin(Guid warehouseId, [FromBody] AddBinRequest request)
    {
        var result = await _mediator.Send(new AddBinCommand(warehouseId, request));
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    /// <summary>Update or deactivate a bin.</summary>
    [HttpPatch("{warehouseId:guid}/bins/{binId:guid}")]
    [Authorize(Roles = "OpsManager,WarehouseManager,SuperAdmin")]
    public async Task<IActionResult> UpdateBin(Guid warehouseId, Guid binId, [FromBody] UpdateBinRequest request)
    {
        var result = await _mediator.Send(new UpdateBinCommand(warehouseId, binId, request));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Receive / Release Operations
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Pre-receive lookup by tracking number (barcode scanner entry point).</summary>
    [HttpGet("lookup/{trackingNumber}")]
    [Authorize(Roles = "WarehouseManager,WarehouseOperator,SuperAdmin")]
    public async Task<IActionResult> LookupShipment(string trackingNumber)
    {
        var result = await _mediator.Send(new LookupShipmentQuery(trackingNumber));
        return Ok(result);
    }

    /// <summary>Receive cargo into a warehouse bin.</summary>
    [HttpPost("receive")]
    [Authorize(Roles = "WarehouseManager,WarehouseOperator,SuperAdmin")]
    public async Task<IActionResult> ReceiveCargo([FromBody] ReceiveCargoRequest request)
    {
        var result = await _mediator.Send(new ReceiveCargoCommand(request, ActorUserId));
        return result.Success
            ? CreatedAtAction(nameof(GetReceipt), new { receiptId = result.Data!.Id }, result)
            : BadRequest(result);
    }

    /// <summary>Release cargo from warehouse for delivery.</summary>
    [HttpPost("release")]
    [Authorize(Roles = "WarehouseManager,WarehouseOperator,SuperAdmin")]
    public async Task<IActionResult> ReleaseCargo([FromBody] ReleaseCargoRequest request)
    {
        var result = await _mediator.Send(new ReleaseCargoCommand(request, ActorUserId));
        return result.Success ? Ok(result) : BadRequest(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Receipts
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Get a single cargo receipt by ID.</summary>
    [HttpGet("receipts/{receiptId:guid}")]
    [Authorize(Roles = "OpsManager,WarehouseManager,WarehouseOperator,SuperAdmin")]
    public async Task<IActionResult> GetReceipt(Guid receiptId)
    {
        var result = await _mediator.Send(new GetReceiptQuery(receiptId));
        return Ok(result);
    }

    /// <summary>List cargo receipts with filters.</summary>
    [HttpGet("receipts")]
    [Authorize(Roles = "OpsManager,WarehouseManager,SuperAdmin")]
    public async Task<IActionResult> GetReceipts(
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] Guid? shipmentId = null,
        [FromQuery] bool? hasDamage = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(
            new GetReceiptsQuery(page, pageSize, warehouseId, shipmentId, hasDamage, dateFrom, dateTo));
        return Ok(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Damage Reports
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>List damage reports with filters.</summary>
    [HttpGet("damage-reports")]
    [Authorize(Roles = "OpsManager,WarehouseManager,SuperAdmin")]
    public async Task<IActionResult> GetDamageReports(
        [FromQuery] Guid? warehouseId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        var result = await _mediator.Send(new GetDamageReportsQuery(warehouseId, status, dateFrom, dateTo));
        return Ok(result);
    }

    /// <summary>Acknowledge or resolve a damage report.</summary>
    [HttpPatch("damage-reports/{reportId:guid}")]
    [Authorize(Roles = "OpsManager,WarehouseManager,SuperAdmin")]
    public async Task<IActionResult> UpdateDamageReport(Guid reportId, [FromBody] UpdateDamageReportRequest request)
    {
        var result = await _mediator.Send(new UpdateDamageReportCommand(reportId, request));
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
