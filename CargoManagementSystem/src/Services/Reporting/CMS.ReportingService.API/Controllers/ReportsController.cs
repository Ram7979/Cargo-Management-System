using CMS.ReportingService.Application.Commands.ExportShipmentReport;
using CMS.ReportingService.Application.DTOs;
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
        [FromQuery] string? format = null)
    {
        var filter = new ShipmentReportFilter
        {
            Status = status,
            CustomerCode = customerCode,
            Origin = origin,
            Destination = destination,
            FromDate = fromDate,
            ToDate = toDate
        };

        if (!string.IsNullOrWhiteSpace(format))
        {
            var exportResult = await _mediator.Send(new ExportShipmentReportCommand(filter, format));
            if (!exportResult.Success)
                return BadRequest(exportResult);

            return Ok(exportResult);
        }

        var result = await _mediator.Send(new GetShipmentReportQuery(page, pageSize, filter));
        return Ok(result);
    }
}
