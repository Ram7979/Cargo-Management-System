using System.Text.Json;
using CMS.ReportingService.Application.Interfaces;
using CMS.ReportingService.Domain.ReadModels;
using Microsoft.Extensions.Logging;

namespace CMS.ReportingService.Infrastructure.Services;

public class ShipmentServiceClient : IShipmentServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ShipmentServiceClient> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

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

            if (!doc.RootElement.TryGetProperty("data", out var dataElement))
                return Enumerable.Empty<ShipmentReadModel>();

            var rawItems = dataElement.ValueKind == JsonValueKind.Array
                ? dataElement
                : dataElement.TryGetProperty("items", out var items) ? items : dataElement;

            var result = new List<ShipmentReadModel>();
            foreach (var element in rawItems.EnumerateArray())
            {
                try
                {
                    var model = new ShipmentReadModel
                    {
                        Id = element.TryGetProperty("id", out var id) ? id.GetGuid() : Guid.NewGuid(),
                        TrackingNumber = element.TryGetProperty("trackingNumber", out var tn) ? tn.GetString() ?? "" : "",
                        CustomerCode = element.TryGetProperty("customerCode", out var cc) ? cc.GetString() ?? "" : "",
                        CustomerName = element.TryGetProperty("customerName", out var cn) ? cn.GetString() ?? "" : "",
                        Status = element.TryGetProperty("status", out var st) ? st.GetString() ?? "" : "",
                        OriginAddress = element.TryGetProperty("originAddress", out var oa) ? oa.GetString() ?? "" : "",
                        OriginCity = element.TryGetProperty("originCity", out var oc) ? oc.GetString() ?? "" : "",
                        OriginCountry = element.TryGetProperty("originCountry", out var ocn) ? ocn.GetString() ?? "" : "",
                        DestinationAddress = element.TryGetProperty("destinationAddress", out var da) ? da.GetString() ?? "" : "",
                        DestinationCity = element.TryGetProperty("destinationCity", out var dc) ? dc.GetString() ?? "" : "",
                        DestinationCountry = element.TryGetProperty("destinationCountry", out var dcn) ? dcn.GetString() ?? "" : "",
                        WeightKg = element.TryGetProperty("weightKg", out var wk) ? wk.GetDecimal() : 0,
                        ServiceType = element.TryGetProperty("serviceType", out var svc) ? svc.GetString() ?? "" : "",
                        CargoType = element.TryGetProperty("cargoType", out var ct) ? ct.GetString() ?? "" : "",
                        DriverId = element.TryGetProperty("driverId", out var di) ? di.GetString() : null,
                        DriverName = element.TryGetProperty("driverName", out var dn) ? dn.GetString() : null,
                        VehicleId = element.TryGetProperty("vehicleId", out var vi) ? vi.GetString() : null,
                        PlateNumber = element.TryGetProperty("plateNumber", out var pn) ? pn.GetString() : null,
                        InvoiceAmount = element.TryGetProperty("invoiceAmount", out var ia) ? ia.GetDecimal() : 0,
                        CreatedAt = element.TryGetProperty("createdAt", out var cat) ? cat.GetDateTime() : DateTime.UtcNow,
                        DeliveredAt = element.TryGetProperty("deliveredAt", out var dat) && dat.ValueKind != JsonValueKind.Null
                            ? dat.GetDateTime() : null,
                        PickedUpAt = element.TryGetProperty("pickedUpAt", out var put) && put.ValueKind != JsonValueKind.Null
                            ? put.GetDateTime() : null
                    };
                    result.Add(model);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse shipment element from Shipment Service response");
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get shipments from Shipment Service (page {Page})", page);
            return Enumerable.Empty<ShipmentReadModel>();
        }
    }
}
