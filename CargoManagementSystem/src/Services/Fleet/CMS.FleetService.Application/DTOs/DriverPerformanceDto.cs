namespace CMS.FleetService.Application.DTOs;

public class DriverPerformanceDto
{
    public Guid DriverId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public int TotalDeliveries { get; set; }
    public int CompletedDeliveries { get; set; }
    public int FailedDeliveries { get; set; }
    public double OnTimeRate { get; set; }
    public double FailureRate { get; set; }
    public int TotalAssignments { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
