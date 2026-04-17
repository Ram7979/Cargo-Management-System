using System.Text;
using ClosedXML.Excel;
using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Application.Interfaces;
using CMS.ReportingService.Domain.Interfaces;
using CMS.ReportingService.Domain.ReadModels;
using CMS.Shared.Responses;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.ReportingService.Application.Commands.ExportShipmentReport;

public class ExportShipmentReportCommandHandler
    : IRequestHandler<ExportShipmentReportCommand, ApiResponse<AsyncExportStatusDto>>
{
    private const int AsyncThreshold = 10_000;

    private readonly IShipmentReadModelRepository _repository;
    private readonly IBlobService _blobService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ILogger<ExportShipmentReportCommandHandler> _logger;

    public ExportShipmentReportCommandHandler(
        IShipmentReadModelRepository repository,
        IBlobService blobService,
        IBackgroundJobClient backgroundJobClient,
        ILogger<ExportShipmentReportCommandHandler> logger)
    {
        _repository = repository;
        _blobService = blobService;
        _backgroundJobClient = backgroundJobClient;
        _logger = logger;
    }

    public async Task<ApiResponse<AsyncExportStatusDto>> Handle(
        ExportShipmentReportCommand request,
        CancellationToken cancellationToken)
    {
        var f = request.Filter;

        // Count first to decide sync vs async
        var (_, totalCount) = await _repository.GetPagedAsync(
            1, 1, f.Status, f.CustomerCode, f.Origin, f.Destination,
            f.FromDate, f.ToDate, f.DriverId, f.VehicleId, f.ServiceType);

        if (totalCount > AsyncThreshold)
        {
            // Dispatch to Hangfire background job
            var jobId = _backgroundJobClient.Enqueue<AsyncExportJob>(
                job => job.ExecuteAsync(request.Filter, request.Format, request.RequestingUserId));

            _logger.LogInformation(
                "Large export ({Count} records) dispatched as background job {JobId} for user {UserId}",
                totalCount, jobId, request.RequestingUserId);

            return ApiResponse<AsyncExportStatusDto>.Ok(
                new AsyncExportStatusDto { JobId = jobId, Status = "Queued" },
                "Export queued. You will be notified by email when the file is ready.");
        }

        // Synchronous export for small datasets
        var records = await _repository.GetAllFilteredAsync(
            f.Status, f.CustomerCode, f.FromDate, f.ToDate,
            f.Origin, f.Destination, f.DriverId, f.VehicleId, f.ServiceType);

        var recordList = records.ToList();
        byte[] fileBytes;
        string fileName;

        if (request.Format.Equals("excel", StringComparison.OrdinalIgnoreCase))
        {
            fileBytes = GenerateExcel(recordList);
            fileName = $"shipment-report-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
        }
        else
        {
            fileBytes = GenerateCsvBytes(recordList);
            fileName = $"shipment-report-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
        }

        var downloadUrl = await _blobService.UploadAsync("reports", fileName, fileBytes);

        return ApiResponse<AsyncExportStatusDto>.Ok(
            new AsyncExportStatusDto
            {
                JobId = Guid.NewGuid().ToString(),
                Status = "Completed",
                DownloadUrl = downloadUrl,
                CompletedAt = DateTime.UtcNow
            },
            "Report exported successfully.");
    }

    internal static byte[] GenerateExcel(List<ShipmentReadModel> records)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Shipments");

        var headers = new[]
        {
            "Tracking Number", "Customer Code", "Customer Name", "Status",
            "Origin", "Destination", "Weight (Kg)", "Service Type",
            "Cargo Type", "Driver", "Plate", "Invoice Amount",
            "Created At", "Delivered At"
        };

        for (int i = 0; i < headers.Length; i++)
            sheet.Cell(1, i + 1).Value = headers[i];

        sheet.Row(1).Style.Font.Bold = true;

        for (int i = 0; i < records.Count; i++)
        {
            var row = i + 2;
            var r = records[i];
            sheet.Cell(row, 1).Value = r.TrackingNumber;
            sheet.Cell(row, 2).Value = r.CustomerCode;
            sheet.Cell(row, 3).Value = r.CustomerName;
            sheet.Cell(row, 4).Value = r.Status;
            sheet.Cell(row, 5).Value = r.OriginAddress;
            sheet.Cell(row, 6).Value = r.DestinationAddress;
            sheet.Cell(row, 7).Value = r.WeightKg;
            sheet.Cell(row, 8).Value = r.ServiceType;
            sheet.Cell(row, 9).Value = r.CargoType;
            sheet.Cell(row, 10).Value = r.DriverName ?? string.Empty;
            sheet.Cell(row, 11).Value = r.PlateNumber ?? string.Empty;
            sheet.Cell(row, 12).Value = r.InvoiceAmount;
            sheet.Cell(row, 13).Value = r.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
            sheet.Cell(row, 14).Value = r.DeliveredAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    internal static byte[] GenerateCsvBytes(List<ShipmentReadModel> records)
    {
        var sb = new StringBuilder();
        sb.AppendLine("TrackingNumber,CustomerCode,CustomerName,Status,Origin,Destination,WeightKg,ServiceType,CargoType,DriverName,PlateNumber,InvoiceAmount,CreatedAt,DeliveredAt");

        foreach (var r in records)
        {
            sb.AppendLine(string.Join(",",
                Escape(r.TrackingNumber), Escape(r.CustomerCode), Escape(r.CustomerName),
                Escape(r.Status), Escape(r.OriginAddress), Escape(r.DestinationAddress),
                r.WeightKg, Escape(r.ServiceType), Escape(r.CargoType),
                Escape(r.DriverName ?? ""), Escape(r.PlateNumber ?? ""),
                r.InvoiceAmount,
                r.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                r.DeliveredAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string Escape(string value)
        => value.Contains(',') || value.Contains('"') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
}
