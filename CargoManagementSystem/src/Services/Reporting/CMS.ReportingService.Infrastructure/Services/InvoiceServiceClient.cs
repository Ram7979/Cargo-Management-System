using System.Text.Json;
using CMS.ReportingService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CMS.ReportingService.Infrastructure.Services;

public class InvoiceServiceClient : IInvoiceServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InvoiceServiceClient> _logger;

    public InvoiceServiceClient(HttpClient httpClient, ILogger<InvoiceServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<int> GetPendingInvoicesCountAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/v1/invoices?status=Issued&pageSize=1");
            if (!response.IsSuccessStatusCode)
                return 0;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("totalCount", out var totalCount))
                return totalCount.GetInt32();

            if (doc.RootElement.TryGetProperty("TotalCount", out var totalCountPascal))
                return totalCountPascal.GetInt32();

            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get pending invoices count from Billing Service");
            return 0;
        }
    }
}
