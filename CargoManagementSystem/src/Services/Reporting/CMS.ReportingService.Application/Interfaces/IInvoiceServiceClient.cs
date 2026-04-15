namespace CMS.ReportingService.Application.Interfaces;

public interface IInvoiceServiceClient
{
    Task<int> GetPendingInvoicesCountAsync();
}
