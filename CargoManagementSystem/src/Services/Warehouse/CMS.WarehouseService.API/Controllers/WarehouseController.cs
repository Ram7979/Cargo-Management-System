using System.Security.Claims;
using CMS.WarehouseService.Application.Commands.ReceiveCargo;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Application.Queries.GetBins;
using CMS.WarehouseService.Application.Queries.GetReceipt;
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

    [HttpPost("receive")]
    [Authorize(Roles = "WarehouseManager,SuperAdmin")]
    public async Task<IActionResult> ReceiveCargo([FromBody] ReceiveCargoRequest request)
    {
        var actorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var result = await _mediator.Send(new ReceiveCargoCommand(request, actorUserId));
        return result.Success
            ? CreatedAtAction(nameof(GetReceipt), new { receiptId = result.Data!.Id }, result)
            : BadRequest(result);
    }

    [HttpGet("receipts/{receiptId:guid}")]
    [Authorize(Roles = "OpsManager,WarehouseManager,SuperAdmin")]
    public async Task<IActionResult> GetReceipt(Guid receiptId)
    {
        var result = await _mediator.Send(new GetReceiptQuery(receiptId));
        return Ok(result);
    }

    [HttpGet("{warehouseId:guid}/bins")]
    [Authorize(Roles = "WarehouseManager,SuperAdmin")]
    public async Task<IActionResult> GetBins(Guid warehouseId)
    {
        var result = await _mediator.Send(new GetBinsQuery(warehouseId));
        return Ok(result);
    }
}
