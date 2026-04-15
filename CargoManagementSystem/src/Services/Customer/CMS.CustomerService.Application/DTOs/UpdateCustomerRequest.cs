namespace CMS.CustomerService.Application.DTOs;

public class UpdateCustomerRequest
{
    public string FullName { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal? CreditLimit { get; set; }
    public string? PaymentTerms { get; set; }
    public string? Type { get; set; }
}
