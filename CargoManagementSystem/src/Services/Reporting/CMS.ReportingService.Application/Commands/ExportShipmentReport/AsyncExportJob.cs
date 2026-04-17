using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Application.Interfaces;
using CMS.ReportingService.Domain.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace CMS.ReportingService.Application.Commands.ExportShipmentReport;

public class AsyncExportJob
{
    private readonly IShipmentReadModelRepository _repository;
    private readonly IBlobService _blobService;
    private readonly INotificationServiceClient _notificationClient;
    private readonly ILogger<AsyncExportJob> _logger;

    public AsyncExportJob(
        IShipmentReadModelRepository repository,
        IBlobService blobService,
        INotificationServiceClient notificationClient,
        ILogger<AsyncExportJob> logger)
    {
        _repository = repository;
        _blobService = blobService;
        _notificationClient = notificationClient;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync(ShipmentReportFilter filter, string format, string requestingUserId)
    {
        _logger.LogInformation("AsyncExportJob started for user {UserId}", requestingUserId);

        var records = await _repository.GetAllFilteredAsync(
            filter.Status, filter.CustomerCode, filter.FromDate, filter.ToDate,
            filter.Origin, filter.Destination, filter.DriverId, filter.VehicleId, filter.ServiceType);

        var recordList = records.ToList();
        byte[] fileBytes;
        string fileName;

        if (format.Equals("excel", StringComparison.OrdinalIgnoreCase))
        {
            fileBytes = ExportShipmentReportCommandHandler.GenerateExcel(recordList);
            fileName = $"shipment-report-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
        }
        else
        {
            fileBytes = ExportShipmentReportCommandHandler.GenerateCsvBytes(recordList);
            fileName = $"shipment-report-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        }

        var downloadUrl = await _blobService.UploadAsync("reports", fileName, fileBytes);

        _logger.LogInformation(
            "AsyncExportJob completed for user {UserId}. File: {FileName}, Records: {Count}",
            requestingUserId, fileName, recordList.Count);

        // Notify user by email
        try
        {
            await _notificationClient.SendReportReadyNotificationAsync(
                requestingUserId, "Shipment Report", downloadUrl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send report-ready notification to user {UserId}", requestingUserId);
        }
    }
}
