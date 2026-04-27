namespace CMS.BillingService.Application.DTOs;

public class RateCardDto
{
    public Guid Id { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string ZoneFrom { get; set; } = string.Empty;
    public string ZoneTo { get; set; } = string.Empty;
    public decimal BaseRatePerKg { get; set; }
    public decimal FuelSurchargePercent { get; set; }
    public decimal HandlingFeeFlat { get; set; }
    public decimal TaxPercent { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public bool IsActive { get; set; }
}

public class CreateRateCardRequest
{
    public string ServiceType { get; set; } = string.Empty;
    public string ZoneFrom { get; set; } = string.Empty;
    public string ZoneTo { get; set; } = string.Empty;
    public decimal BaseRatePerKg { get; set; }
    public decimal FuelSurchargePercent { get; set; }
    public decimal HandlingFeeFlat { get; set; }
    public decimal TaxPercent { get; set; } = 18m;
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
}

public class RateCalculationResult
{
    public decimal BaseFreightCharge { get; set; }
    public decimal FuelSurcharge { get; set; }
    public decimal HandlingFee { get; set; }
    public decimal InsuranceAmount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public string ServiceType { get; set; } = string.Empty;
    public int EstimatedDays { get; set; }
}

public class VoidInvoiceRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class MarkPaidRequest
{
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class RefundPaymentRequest
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Request body for PUT /api/v1/rates/{rateCardId}.
/// Updates pricing fields on an existing rate card.
/// ServiceType, ZoneFrom, ZoneTo are immutable — create a new card to change those.
/// </summary>
public class UpdateRateCardRequest
{
    /// <summary>New base rate per kilogram of chargeable weight.</summary>
    public decimal BaseRatePerKg { get; set; }

    /// <summary>Fuel surcharge as a percentage of base freight (e.g. 8 = 8%).</summary>
    public decimal FuelSurchargePercent { get; set; }

    /// <summary>Flat handling fee added to every shipment.</summary>
    public decimal HandlingFeeFlat { get; set; }

    /// <summary>Tax percentage applied to the subtotal (e.g. 18 = 18% GST).</summary>
    public decimal TaxPercent { get; set; } = 18m;

    /// <summary>Optional end date for this rate card. Null means no expiry.</summary>
    public DateTime? EffectiveTo { get; set; }
}
