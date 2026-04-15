using System.Text.Json;
using CMS.CustomerService.Application.DTOs;
using CMS.CustomerService.Application.Interfaces;
using CMS.Shared.Responses;
using Microsoft.Extensions.Logging;

namespace CMS.CustomerService.Infrastructure.Services;

public class ShipmentServiceClient : IShipmentServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ShipmentServiceClient> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ShipmentServiceClient(HttpClient httpClient, ILogger<ShipmentServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<(IEnumerable<CustomerShipmentDto> Items, int TotalCount)> GetCustomerShipmentsAsync(
        Guid customerId, int page, int pageSize,
        string? status = null, string? serviceType = null,
        DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        try
        {
            var url = $"api/v1/shipments?customerId={customerId}&page={page}&pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(status)) url += $"&status={status}";
            if (!string.IsNullOrWhiteSpace(serviceType)) url += $"&serviceType={serviceType}";
            if (dateFrom.HasValue) url += $"&dateFrom={dateFrom.Value:yyyy-MM-dd}";
            if (dateTo.HasValue) url += $"&dateTo={dateTo.Value:yyyy-MM-dd}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return (Enumerable.Empty<CustomerShipmentDto>(), 0);

            var content = await response.Content.ReadAsStringAsync();
            var paged = JsonSerializer.Deserialize<PagedResponse<CustomerShipmentDto>>(content, _jsonOptions);

            if (paged?.Data is null)
                return (Enumerable.Empty<CustomerShipmentDto>(), 0);

            return (paged.Data, paged.TotalCount);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to fetch shipments for customer {CustomerId}", customerId);
            return (Enumerable.Empty<CustomerShipmentDto>(), 0);
        }
    }
}
