using CMS.ReportingService.Application.Queries.GetDashboardKpis;
using CMS.ReportingService.Application.Queries.GetDashboardSummary;
using CMS.ReportingService.Application.Queries.GetShipmentReport;
using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Application.Queries.GetTrendData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.ReportingService.API.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Dashboard summary with all KPI values.</summary>
    [HttpGet("summary")]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetSummary(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var result = await _mediator.Send(new GetDashboardSummaryQuery(fromDate, toDate));
        return Ok(result);
    }

    /// <summary>Individual KPI cards for the dashboard.</summary>
    [HttpGet("kpis")]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetKpis(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var result = await _mediator.Send(new GetDashboardKpisQuery(fromDate, toDate));
        return Ok(result);
    }

    /// <summary>Time-series data for charts: revenue trend, status over time, cargo types, geo heat map.</summary>
    [HttpGet("trend-data")]
    [Authorize(Roles = "OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetTrendData(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string groupBy = "day")
    {
        var from = fromDate ?? DateTime.UtcNow.AddDays(-30);
        var to = toDate ?? DateTime.UtcNow;
        var result = await _mediator.Send(new GetTrendDataQuery(from, to, groupBy));
        return Ok(result);
    }

    /// <summary>KPI drill-down: list of shipments for a given status.</summary>
    [HttpGet("shipments")]
    [Authorize(Roles = "OpsManager,FinanceOfficer,SuperAdmin")]
    public async Task<IActionResult> GetShipmentDrillDown(
        [FromQuery] string? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var filter = new ShipmentReportFilter
        {
            Status = status,
            FromDate = fromDate,
            ToDate = toDate
        };
        var result = await _mediator.Send(new GetShipmentReportQuery(page, pageSize, filter));
        return Ok(result);
    }
}
