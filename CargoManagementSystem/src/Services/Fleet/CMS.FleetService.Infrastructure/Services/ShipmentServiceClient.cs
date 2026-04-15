using System.Text;
using System.Text.Json;
using CMS.FleetService.Application.Interfaces;

namespace CMS.FleetService.Infrastructure.Services;

public class ShipmentServiceClient : IShipmentServiceClient
{
    private readonly HttpClient _httpClient;

    public ShipmentServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task UpdateShipmentStatusAsync(Guid shipmentId, string status, string actorId)
    {
        var payload = new { status, notes = "Assigned by Fleet Service" };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PatchAsync($"/api/v1/shipments/{shipmentId}/status", content);
        response.EnsureSuccessStatusCode();
    }
}
