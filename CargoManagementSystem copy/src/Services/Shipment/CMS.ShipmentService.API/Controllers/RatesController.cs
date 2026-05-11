using CMS.ShipmentService.Application.Queries.CalculateRate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.ShipmentService.API.Controllers;

[ApiController]
[Route("api/v1/rates")]
public class RatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Calculate freight rate before confirming shipment creation.
    /// </summary>
    [HttpGet("calculate")]
    [AllowAnonymous]
    public async Task<IActionResult> Calculate(
        [FromQuery] decimal weightKg,
        [FromQuery] decimal volumeCbm,
        [FromQuery] string serviceType,
        [FromQuery] string originCountry,
        [FromQuery] string destinationCountry)
    {
        var result = await _mediator.Send(new CalculateRateQuery(
            weightKg, volumeCbm, serviceType, originCountry, destinationCountry));
        return Ok(result);
    }
}
