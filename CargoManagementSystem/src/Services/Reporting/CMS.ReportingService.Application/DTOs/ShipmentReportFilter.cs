namespace CMS.ReportingService.Application.DTOs;

public class ShipmentReportFilter
{
    public string? Status { get; set; }
    public string? CustomerCode { get; set; }
    public string? Origin { get; set; }
    public string? Destination { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
