using CMS.ReportingService.Application.Interfaces;
using CMS.ReportingService.Domain.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace CMS.ReportingService.Application.Jobs;

public class SyncShipmentReadModelsJob
{
    private readonly IShipmentReadModelRepository _repository;
    private readonly IShipmentServiceClient _shipmentServiceClient;
    private readonly ILogger<SyncShipmentReadModelsJob> _logger;

    public SyncShipmentReadModelsJob(
        IShipmentReadModelRepository repository,
        IShipmentServiceClient shipmentServiceClient,
        ILogger<SyncShipmentReadModelsJob> logger)
    {
        _repository = repository;
        _shipmentServiceClient = shipmentServiceClient;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task Execute()
    {
        _logger.LogInformation("SyncShipmentReadModelsJob started at {Time}", DateTime.UtcNow);

        int page = 1;
        const int pageSize = 100;
        int totalSynced = 0;

        while (true)
        {
            var shipments = (await _shipmentServiceClient.GetRecentShipmentsAsync(page, pageSize)).ToList();

            if (!shipments.Any())
                break;

            foreach (var shipment in shipments)
            {
                try
                {
                    await _repository.AddOrUpdateAsync(shipment);
                    totalSynced++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Failed to sync shipment {TrackingNumber}", shipment.TrackingNumber);
                }
            }

            if (shipments.Count < pageSize)
                break;

            page++;
        }

        _logger.LogInformation(
            "SyncShipmentReadModelsJob completed at {Time}. Synced {Count} records.",
            DateTime.UtcNow, totalSynced);
    }
}
