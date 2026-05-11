using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Queries.CalculateRate;

public class CalculateRateQueryHandler : IRequestHandler<CalculateRateQuery, ApiResponse<RateCalculationResult>>
{
    private readonly IRateCardRepository _rateCardRepository;

    public CalculateRateQueryHandler(IRateCardRepository rateCardRepository)
    {
        _rateCardRepository = rateCardRepository;
    }

    public async Task<ApiResponse<RateCalculationResult>> Handle(CalculateRateQuery query, CancellationToken cancellationToken)
    {
        // Try to find a matching rate card
        var rateCard = await _rateCardRepository.GetBestMatchAsync(
            query.ServiceType, query.OriginCountry, query.DestinationCountry);

        decimal baseRatePerKg, fuelSurchargePercent, handlingFeeFlat, taxPercent;

        if (rateCard != null)
        {
            baseRatePerKg = rateCard.BaseRatePerKg;
            fuelSurchargePercent = rateCard.FuelSurchargePercent;
            handlingFeeFlat = rateCard.HandlingFeeFlat;
            taxPercent = rateCard.TaxPercent;
        }
        else
        {
            // Default rates when no rate card configured
            baseRatePerKg = query.ServiceType.ToLower() switch
            {
                "express" => 15.0m,
                "same-day" => 25.0m,
                "international" => 30.0m,
                _ => 8.0m
            };
            fuelSurchargePercent = 8m;
            handlingFeeFlat = query.ServiceType.ToLower() == "international" ? 25.0m : 10.0m;
            taxPercent = 18m;
        }

        // Chargeable weight = max(actual weight, volumetric weight)
        var volumetricWeight = query.VolumeCbm * 167m;
        var chargeableWeight = Math.Max(query.WeightKg, volumetricWeight);

        var baseFreight = Math.Round(chargeableWeight * baseRatePerKg, 2);
        var fuelSurcharge = Math.Round(baseFreight * (fuelSurchargePercent / 100), 2);
        var handlingFee = handlingFeeFlat;
        var insurance = query.CargoType?.ToLower() == "hazardous" ? Math.Round(baseFreight * 0.02m, 2) : 0m;
        var subtotal = baseFreight + fuelSurcharge + handlingFee + insurance;
        var taxAmount = Math.Round(subtotal * (taxPercent / 100), 2);
        var total = subtotal + taxAmount;

        var estimatedDays = query.ServiceType.ToLower() switch
        {
            "same-day" => 0,
            "express" => 1,
            "international" => 7,
            _ => 3
        };

        var result = new RateCalculationResult
        {
            BaseFreightCharge = baseFreight,
            FuelSurcharge = fuelSurcharge,
            HandlingFee = handlingFee,
            InsuranceAmount = insurance,
            TaxRate = taxPercent / 100,
            TaxAmount = taxAmount,
            TotalAmount = total,
            Currency = "USD",
            ServiceType = query.ServiceType,
            EstimatedDays = estimatedDays
        };

        return ApiResponse<RateCalculationResult>.Ok(result, "Rate calculated successfully.");
    }
}
