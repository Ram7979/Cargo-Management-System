using System.Text;
using System.Text.Json;
using CMS.WarehouseService.Application.Interfaces;

namespace CMS.WarehouseService.Infrastructure.Services;

public class ShipmentServiceClient : IShipmentServiceClient
{
    private readonly HttpClient _httpClient;

    public ShipmentServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetShipmentStatusAsync(Guid shipmentId)
    {
        var response = await _httpClient.GetAsync($"/api/v1/shipments/{shipmentId}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);

        // Response envelope: { success, data: { status, ... } }
        var status = doc.RootElement
            .GetProperty("data")
            .GetProperty("status")
            .GetString();

        return status ?? string.Empty;
    }

    public async Task UpdateShipmentStatusAsync(Guid shipmentId, string status, string actorId)
    {
        var payload = new { status, notes = "Updated by Warehouse Service", actorId };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PatchAsync($"/api/v1/shipments/{shipmentId}/status", content);
        response.EnsureSuccessStatusCode();
    }
}
