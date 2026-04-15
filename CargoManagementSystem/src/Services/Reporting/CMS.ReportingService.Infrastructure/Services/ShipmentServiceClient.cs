using System.Text.Json;
using CMS.ReportingService.Domain.ReadModels;
using Microsoft.Extensions.Logging;

namespace CMS.ReportingService.Infrastructure.Services;

public class ShipmentServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ShipmentServiceClient> _logger;

    public ShipmentServiceClient(HttpClient httpClient, ILogger<ShipmentServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<ShipmentReadModel>> GetRecentShipmentsAsync(int page = 1, int pageSize = 100)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/v1/shipments?page={page}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<ShipmentReadModel>();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            // Parse the data array from the paged response
            if (!doc.RootElement.TryGetProperty("data", out var dataElement))
                return Enumerable.Empty<ShipmentReadModel>();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var shipments = JsonSerializer.Deserialize<List<ShipmentReadModel>>(dataElement.GetRawText(), options);
            return shipments ?? Enumerable.Empty<ShipmentReadModel>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get shipments from Shipment Service");
            return Enumerable.Empty<ShipmentReadModel>();
        }
    }
}
