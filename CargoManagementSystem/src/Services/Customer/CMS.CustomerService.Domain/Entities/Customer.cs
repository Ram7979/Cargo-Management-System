using CMS.Shared.Entities;
using CMS.CustomerService.Domain.Enums;

namespace CMS.CustomerService.Domain.Entities;

public class Customer : BaseEntity
{
    public string CustomerCode { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string CompanyName { get; private set; } = string.Empty;
    public string ContactPerson { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string ZipCode { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string TaxId { get; private set; } = string.Empty;
    public CustomerType Type { get; private set; }
    public decimal CreditLimit { get; private set; }
    public string PaymentTerms { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;

    private readonly List<KycDocument> _kycDocuments = new();
    public ICollection<KycDocument> KycDocuments => _kycDocuments.AsReadOnly();

    private Customer() { }

    public static Customer Create(
        string customerCode,
        string fullName,
        string email,
        string phone,
        string address,
        string city,
        string country,
        CustomerType type,
        string companyName = "",
        string contactPerson = "",
        string state = "",
        string zipCode = "",
        string taxId = "",
        decimal creditLimit = 0,
        string paymentTerms = "")
    {
        return new Customer
        {
            CustomerCode = customerCode,
            FullName = fullName,
            CompanyName = companyName,
            ContactPerson = contactPerson,
            Email = email,
            Phone = phone,
            Address = address,
            City = city,
            State = state,
            ZipCode = zipCode,
            Country = country,
            TaxId = taxId,
            Type = type,
            CreditLimit = creditLimit,
            PaymentTerms = paymentTerms,
            IsActive = true
        };
    }

    public void Update(
        string fullName, string phone, string address,
        string city, string country,
        string companyName = "", string contactPerson = "",
        string state = "", string zipCode = "",
        decimal? creditLimit = null, string? paymentTerms = null)
    {
        FullName = fullName;
        Phone = phone;
        Address = address;
        City = city;
        Country = country;
        if (!string.IsNullOrWhiteSpace(companyName)) CompanyName = companyName;
        if (!string.IsNullOrWhiteSpace(contactPerson)) ContactPerson = contactPerson;
        if (!string.IsNullOrWhiteSpace(state)) State = state;
        if (!string.IsNullOrWhiteSpace(zipCode)) ZipCode = zipCode;
        if (creditLimit.HasValue) CreditLimit = creditLimit.Value;
        if (paymentTerms != null) PaymentTerms = paymentTerms;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateType(CustomerType type)
    {
        Type = type;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddKycDocument(string documentType, string blobReference)
    {
        var doc = KycDocument.Create(Id, documentType, blobReference);
        _kycDocuments.Add(doc);
    }

    public void RemoveKycDocument(Guid documentId)
    {
        var doc = _kycDocuments.FirstOrDefault(d => d.Id == documentId);
        if (doc != null)
            _kycDocuments.Remove(doc);
    }
}
