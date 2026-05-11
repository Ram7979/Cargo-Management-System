using System.Text.Json;
using CMS.BillingService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CMS.BillingService.Infrastructure.Services;

public class CustomerServiceClient : ICustomerServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CustomerServiceClient> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public CustomerServiceClient(HttpClient httpClient, ILogger<CustomerServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CustomerPaymentTerms?> GetCustomerPaymentTermsAsync(string customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/v1/customers/{customerId}");
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(content);
            var data = doc.RootElement.GetProperty("data");

            return new CustomerPaymentTerms
            {
                PaymentTerms = data.TryGetProperty("paymentTerms", out var pt) ? pt.GetString() ?? "" : "",
                CreditLimit = data.TryGetProperty("creditLimit", out var cl) ? cl.GetDecimal() : 0,
                CustomerType = data.TryGetProperty("type", out var t) ? t.GetString() ?? "" : ""
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch customer {CustomerId} payment terms", customerId);
            return null;
        }
    }
}
