using System.Text;
using ClosedXML.Excel;
using CMS.ReportingService.Application.Interfaces;
using CMS.ReportingService.Domain.Interfaces;
using CMS.ReportingService.Domain.ReadModels;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Commands.ExportShipmentReport;

public class ExportShipmentReportCommandHandler
    : IRequestHandler<ExportShipmentReportCommand, ApiResponse<string>>
{
    private readonly IShipmentReadModelRepository _repository;
    private readonly IBlobService _blobService;

    public ExportShipmentReportCommandHandler(
        IShipmentReadModelRepository repository,
        IBlobService blobService)
    {
        _repository = repository;
        _blobService = blobService;
    }

    public async Task<ApiResponse<string>> Handle(
        ExportShipmentReportCommand request,
        CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        var records = await _repository.GetAllFilteredAsync(
            filter.Status, filter.CustomerCode, filter.FromDate, filter.ToDate);

        var recordList = records.ToList();
        byte[] fileBytes;
        string fileName;
        string containerName = "reports";

        if (request.Format.Equals("excel", StringComparison.OrdinalIgnoreCase))
        {
            fileBytes = GenerateExcel(recordList);
            fileName = $"shipment-report-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx";
        }
        else
        {
            fileBytes = GeneratePdf(recordList);
            fileName = $"shipment-report-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf";
        }

        var downloadUrl = await _blobService.UploadAsync(containerName, fileName, fileBytes);

        return ApiResponse<string>.Ok(downloadUrl, "Report exported successfully.");
    }

    private static byte[] GenerateExcel(List<ShipmentReadModel> records)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Shipments");

        // Headers
        sheet.Cell(1, 1).Value = "Tracking Number";
        sheet.Cell(1, 2).Value = "Customer Code";
        sheet.Cell(1, 3).Value = "Customer Name";
        sheet.Cell(1, 4).Value = "Status";
        sheet.Cell(1, 5).Value = "Origin";
        sheet.Cell(1, 6).Value = "Destination";
        sheet.Cell(1, 7).Value = "Weight (Kg)";
        sheet.Cell(1, 8).Value = "Service Type";
        sheet.Cell(1, 9).Value = "Created At";
        sheet.Cell(1, 10).Value = "Delivered At";

        var headerRow = sheet.Row(1);
        headerRow.Style.Font.Bold = true;

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
            sheet.Cell(row, 9).Value = r.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
            sheet.Cell(row, 10).Value = r.DeliveredAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static byte[] GeneratePdf(List<ShipmentReadModel> records)
    {
        var sb = new StringBuilder();
        sb.AppendLine("SHIPMENT REPORT");
        sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine(new string('-', 80));
        sb.AppendLine($"{"Tracking",-20} {"Customer",-15} {"Status",-20} {"Weight",8}");
        sb.AppendLine(new string('-', 80));

        foreach (var r in records)
        {
            sb.AppendLine($"{r.TrackingNumber,-20} {r.CustomerCode,-15} {r.Status,-20} {r.WeightKg,8:F2}");
        }

        sb.AppendLine(new string('-', 80));
        sb.AppendLine($"Total records: {records.Count}");

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
