namespace CMS.ReportingService.Application.Interfaces;

public interface INotificationServiceClient
{
    Task SendReportReadyNotificationAsync(string userId, string reportType, string downloadUrl);
}
