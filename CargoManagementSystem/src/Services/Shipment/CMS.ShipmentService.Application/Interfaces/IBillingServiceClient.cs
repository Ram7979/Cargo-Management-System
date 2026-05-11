using System;
using System.Threading.Tasks;
using CMS.Shared.Responses;

namespace CMS.ShipmentService.Application.Interfaces;

public class GenerateInvoiceInternalRequest
{
    public Guid ShipmentId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public decimal BaseFreightCharge { get; set; }
    public decimal FuelSurcharge { get; set; }
    public decimal HandlingFee { get; set; }
    public decimal InsuranceAmount { get; set; }
    public decimal TaxRate { get; set; }
    public DateTime? DueDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public interface IBillingServiceClient
{
    Task<bool> GenerateInvoiceAsync(GenerateInvoiceInternalRequest request);
}
