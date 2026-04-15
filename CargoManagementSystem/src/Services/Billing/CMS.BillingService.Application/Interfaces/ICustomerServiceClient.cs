namespace CMS.BillingService.Application.Interfaces;

public interface ICustomerServiceClient
{
    Task<CustomerPaymentTerms?> GetCustomerPaymentTermsAsync(Guid customerId);
}

public class CustomerPaymentTerms
{
    public string PaymentTerms { get; set; } = string.Empty; // e.g. "Net 30"
    public decimal CreditLimit { get; set; }
    public string CustomerType { get; set; } = string.Empty;
}
