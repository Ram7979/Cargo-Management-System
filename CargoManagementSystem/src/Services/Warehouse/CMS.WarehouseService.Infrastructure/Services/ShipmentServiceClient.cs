using System.Text;
using System.Text.Json;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Application.Interfaces;

namespace CMS.WarehouseService.Infrastructure.Services;

public class ShipmentServiceClient : IShipmentServiceClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

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

        var status = doc.RootElement
            .GetProperty("data")
            .GetProperty("status")
            .GetString();

        return status ?? string.Empty;
    }

    public async Task<ShipmentLookupDto?> GetShipmentByTrackingNumberAsync(string trackingNumber)
    {
        var response = await _httpClient.GetAsync($"/api/v1/shipments/track/{Uri.EscapeDataString(trackingNumber)}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);

        var data = doc.RootElement.GetProperty("data");

        return new ShipmentLookupDto
        {
            ShipmentId = data.GetProperty("id").GetGuid(),
            TrackingNumber = data.GetProperty("trackingNumber").GetString() ?? string.Empty,
            SenderName = data.TryGetProperty("senderName", out var sn) ? sn.GetString() ?? string.Empty : string.Empty,
            RecipientName = data.TryGetProperty("recipientName", out var rn) ? rn.GetString() ?? string.Empty : string.Empty,
            CargoDescription = data.TryGetProperty("cargoDescription", out var cd) ? cd.GetString() ?? string.Empty : string.Empty,
            WeightKg = data.TryGetProperty("weightKg", out var wk) ? wk.GetDecimal() : 0,
            VolumeCbm = data.TryGetProperty("volumeCbm", out var vc) ? vc.GetDecimal() : 0,
            Status = data.GetProperty("status").GetString() ?? string.Empty,
            OriginAddress = data.TryGetProperty("originAddress", out var oa) ? oa.GetString() ?? string.Empty : string.Empty,
            DestinationAddress = data.TryGetProperty("destinationAddress", out var da) ? da.GetString() ?? string.Empty : string.Empty
        };
    }

    public async Task UpdateShipmentStatusAsync(Guid shipmentId, string status, string actorId)
    {
        var payload = new { status, notes = "Updated by Warehouse Service", actorId };
        var content = new StringContent(
            JsonSerializer.Serialize(payload, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PatchAsync($"/api/v1/shipments/{shipmentId}/status", content);
        response.EnsureSuccessStatusCode();
    }
}
