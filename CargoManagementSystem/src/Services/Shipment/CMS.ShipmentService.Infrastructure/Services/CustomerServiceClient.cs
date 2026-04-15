using CMS.ShipmentService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CMS.ShipmentService.Infrastructure.Services;

public class CustomerServiceClient : ICustomerServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CustomerServiceClient> _logger;

    public CustomerServiceClient(HttpClient httpClient, ILogger<CustomerServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> CustomerExistsAsync(Guid customerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/v1/customers/{customerId}");

            if (response.IsSuccessStatusCode)
                return true;

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            // Unexpected status — log and assume customer exists to avoid blocking shipment creation
            _logger.LogWarning(
                "Customer service returned unexpected status {Status} for customer {CustomerId}. Proceeding.",
                (int)response.StatusCode, customerId);
            return true;
        }
        catch (HttpRequestException ex)
        {
            // Customer service is down — log and allow shipment creation to proceed
            _logger.LogWarning(ex,
                "Customer service is unavailable. Skipping customer validation for {CustomerId}.",
                customerId);
            return true;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex,
                "Customer service request timed out for {CustomerId}. Skipping validation.",
                customerId);
            return true;
        }
    }
}
