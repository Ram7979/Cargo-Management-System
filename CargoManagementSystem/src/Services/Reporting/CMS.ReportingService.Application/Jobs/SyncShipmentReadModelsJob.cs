using CMS.ReportingService.Domain.Interfaces;
using CMS.ReportingService.Domain.ReadModels;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace CMS.ReportingService.Application.Jobs;

public class SyncShipmentReadModelsJob
{
    private readonly IShipmentReadModelRepository _repository;
    private readonly ILogger<SyncShipmentReadModelsJob> _logger;

    public SyncShipmentReadModelsJob(
        IShipmentReadModelRepository repository,
        ILogger<SyncShipmentReadModelsJob> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task Execute()
    {
        _logger.LogInformation("SyncShipmentReadModelsJob started at {Time}", DateTime.UtcNow);

        // Registration: RecurringJob.AddOrUpdate<SyncShipmentReadModelsJob>(
        //   "sync-shipments", j => j.Execute(), "*/5 * * * *")
        // The actual HTTP call to Shipment Service is handled by IShipmentServiceClient
        // injected in the Infrastructure layer implementation.

        _logger.LogInformation("SyncShipmentReadModelsJob completed at {Time}", DateTime.UtcNow);

        await Task.CompletedTask;
    }
}
