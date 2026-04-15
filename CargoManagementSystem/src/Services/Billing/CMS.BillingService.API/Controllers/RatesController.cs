using CMS.BillingService.Application.Commands.CreateRateCard;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Application.Queries.CalculateRate;
using CMS.BillingService.Application.Queries.GetRateCards;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.BillingService.API.Controllers;

[ApiController]
[Route("api/v1/rates")]
public class RatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Calculate estimated freight charges before shipment creation.</summary>
    [HttpGet("calculate")]
    [AllowAnonymous]
    public async Task<IActionResult> Calculate(
        [FromQuery] decimal weightKg,
        [FromQuery] decimal volumeCbm,
        [FromQuery] string serviceType,
        [FromQuery] string originCountry,
        [FromQuery] string destinationCountry,
        [FromQuery] string? cargoType = null)
    {
        var result = await _mediator.Send(new CalculateRateQuery(
            weightKg, volumeCbm, serviceType, originCountry, destinationCountry, cargoType));
        return Ok(result);
    }

    /// <summary>List all rate cards.</summary>
    [HttpGet]
    [Authorize(Roles = "FinanceOfficer,OpsManager,SuperAdmin")]
    public async Task<IActionResult> GetRateCards(
        [FromQuery] string? serviceType = null,
        [FromQuery] bool activeOnly = true)
    {
        var result = await _mediator.Send(new GetRateCardsQuery(serviceType, activeOnly));
        return Ok(result);
    }

    /// <summary>Create a new rate card.</summary>
    [HttpPost]
    [Authorize(Roles = "FinanceOfficer,OpsManager,SuperAdmin")]
    public async Task<IActionResult> CreateRateCard([FromBody] CreateRateCardRequest request)
    {
        var result = await _mediator.Send(new CreateRateCardCommand(request));
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }
}
