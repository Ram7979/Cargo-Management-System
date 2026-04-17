using System.Security.Claims;
using CMS.ReportingService.Application.Commands.ExportShipmentReport;
using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Application.Queries.GetCustomerStatement;
using CMS.ReportingService.Application.Queries.GetFleetReport;
using CMS.ReportingService.Application.Queries.GetRevenueReport;
using CMS.ReportingService.Application.Queries.GetShipmentReport;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.ReportingService.API.Controllers;

[ApiController]
[Route("api/v1/reports")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private string ActorUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

    // ─────────────────────────────────────────────────────────────────────────
    // Shipment Report
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Paginated shipment report with full filter set.</summary>
    [HttpGet("shipments")]
    [Authorize(Roles = "OpsManager,FinanceOfficer,SuperAdmin")]
    public async Task<IActionResult> GetShipmentReport(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? status = null,
        [FromQuery] string? customerCode = null,
        [FromQuery] string? origin = null,
        [FromQuery] string? destination = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? driverId = null,
        [FromQuery] string? vehicleId = null,
        [FromQuery] string? serviceType = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDir = null)
    {
        var filter = new ShipmentReportFilter
        {
            Status = status,
            CustomerCode = customerCode,
            Origin = origin,
            Destination = destination,
            FromDate = fromDate,
            ToDate = toDate,
            DriverId = driverId,
            VehicleId = vehicleId,
            ServiceType = serviceType,
            SortBy = sortBy,
            SortDir = sortDir
        };

        var result = await _mediator.Send(new GetShipmentReportQuery(page, pageSize, filter));
        return Ok(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Revenue Report
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Revenue report with invoiced/collected/outstanding breakdown.</summary>
    [HttpGet("revenue")]
    [Authorize(Roles = "FinanceOfficer,OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetRevenueReport(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? customerId = null,
        [FromQuery] string? serviceType = null,
        [FromQuery] string? groupBy = null)
    {
        var from = fromDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = toDate ?? DateTime.UtcNow;
        var result = await _mediator.Send(new GetRevenueReportQuery(from, to, customerId, serviceType, groupBy));
        return Ok(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Fleet Report
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Fleet performance: deliveries per driver, on-time rate, failed rate.</summary>
    [HttpGet("fleet")]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetFleetReport(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? driverId = null,
        [FromQuery] string? vehicleId = null)
    {
        var from = fromDate ?? DateTime.UtcNow.AddMonths(-1);
        var to = toDate ?? DateTime.UtcNow;
        var result = await _mediator.Send(new GetFleetReportQuery(from, to, driverId, vehicleId));
        return Ok(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Customer Statement
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Consolidated customer statement: invoices, payments, balance, shipment history.</summary>
    [HttpGet("customers/{customerId}/statement")]
    [Authorize(Roles = "FinanceOfficer,OpsManager,SuperAdmin,Customer")]
    public async Task<IActionResult> GetCustomerStatement(
        string customerId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var from = fromDate ?? DateTime.UtcNow.AddMonths(-3);
        var to = toDate ?? DateTime.UtcNow;
        var result = await _mediator.Send(new GetCustomerStatementQuery(customerId, from, to));
        return Ok(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Async Export
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Export report. Returns file URL synchronously for &lt;10k records.
    /// Returns 202 with jobId for larger datasets — email sent on completion.
    /// </summary>
    [HttpPost("export")]
    [Authorize(Roles = "OpsManager,FinanceOfficer,SuperAdmin")]
    public async Task<IActionResult> ExportReport([FromBody] AsyncExportRequestDto request)
    {
        var filter = request.Filters ?? new ShipmentReportFilter();
        var result = await _mediator.Send(
            new ExportShipmentReportCommand(filter, request.Format, ActorUserId));

        if (!result.Success)
            return BadRequest(result);

        // 202 Accepted for async jobs, 200 for sync completions
        return result.Data?.Status == "Queued"
            ? Accepted(result)
            : Ok(result);
    }

    /// <summary>Poll async export job status.</summary>
    [HttpGet("export/{jobId}")]
    [Authorize(Roles = "OpsManager,FinanceOfficer,SuperAdmin")]
    public IActionResult GetExportStatus(string jobId)
    {
        // Hangfire job status polling — returns basic status
        // In production this would query Hangfire's monitoring API
        return Ok(new AsyncExportStatusDto
        {
            JobId = jobId,
            Status = "Processing"
        });
    }
}
