using CMS.Shared.Entities;

namespace CMS.BillingService.Domain.Entities;

public class RateCard : BaseEntity
{
    public string ServiceType { get; private set; } = string.Empty;
    public string ZoneFrom { get; private set; } = string.Empty;
    public string ZoneTo { get; private set; } = string.Empty;
    public decimal BaseRatePerKg { get; private set; }
    public decimal FuelSurchargePercent { get; private set; }
    public decimal HandlingFeeFlat { get; private set; }
    public decimal TaxPercent { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public bool IsActive { get; private set; }

    private RateCard() { }

    public static RateCard Create(
        string serviceType, string zoneFrom, string zoneTo,
        decimal baseRatePerKg, decimal fuelSurchargePercent,
        decimal handlingFeeFlat, decimal taxPercent,
        DateTime effectiveFrom, DateTime? effectiveTo = null)
    {
        return new RateCard
        {
            ServiceType = serviceType,
            ZoneFrom = zoneFrom,
            ZoneTo = zoneTo,
            BaseRatePerKg = baseRatePerKg,
            FuelSurchargePercent = fuelSurchargePercent,
            HandlingFeeFlat = handlingFeeFlat,
            TaxPercent = taxPercent,
            EffectiveFrom = effectiveFrom,
            EffectiveTo = effectiveTo,
            IsActive = true
        };
    }

    public void Update(decimal baseRatePerKg, decimal fuelSurchargePercent,
        decimal handlingFeeFlat, decimal taxPercent, DateTime? effectiveTo)
    {
        BaseRatePerKg = baseRatePerKg;
        FuelSurchargePercent = fuelSurchargePercent;
        HandlingFeeFlat = handlingFeeFlat;
        TaxPercent = taxPercent;
        EffectiveTo = effectiveTo;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
