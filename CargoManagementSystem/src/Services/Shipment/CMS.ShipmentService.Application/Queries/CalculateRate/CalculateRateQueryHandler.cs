using CMS.ShipmentService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Queries.CalculateRate;

public class CalculateRateQueryHandler : IRequestHandler<CalculateRateQuery, ApiResponse<RateCalculationDto>>
{
    public Task<ApiResponse<RateCalculationDto>> Handle(CalculateRateQuery query, CancellationToken cancellationToken)
    {
        // Rate calculation engine — simplified formula
        // In production: integrate with a rate card table or external pricing API
        var isInternational = !string.Equals(query.OriginCountry, query.DestinationCountry, StringComparison.OrdinalIgnoreCase);

        decimal baseRate = query.ServiceType.ToLower() switch
        {
            "express" => 15.0m,
            "same-day" => 25.0m,
            "international" => 30.0m,
            _ => 8.0m  // Standard
        };

        var baseFreight = Math.Max(query.WeightKg, query.VolumeCbm * 167) * baseRate;
        var fuelSurcharge = baseFreight * 0.08m;
        var handlingFee = isInternational ? 25.0m : 10.0m;
        var subtotal = baseFreight + fuelSurcharge + handlingFee;
        var tax = subtotal * 0.18m;
        var total = subtotal + tax;

        var estimatedDays = query.ServiceType.ToLower() switch
        {
            "same-day" => 0,
            "express" => 1,
            "international" => 7,
            _ => 3
        };

        var dto = new RateCalculationDto
        {
            BaseFreightCharge = Math.Round(baseFreight, 2),
            FuelSurcharge = Math.Round(fuelSurcharge, 2),
            HandlingFee = Math.Round(handlingFee, 2),
            TaxAmount = Math.Round(tax, 2),
            TotalAmount = Math.Round(total, 2),
            Currency = "USD",
            ServiceType = query.ServiceType,
            EstimatedDays = estimatedDays
        };

        return Task.FromResult(ApiResponse<RateCalculationDto>.Ok(dto, "Rate calculated successfully."));
    }
}
