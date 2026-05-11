using System.Text.Json;
using CMS.ReportingService.Application.DTOs;
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
            if (!response.IsSuccessStatusCode) return 0;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.TryGetProperty("totalCount", out var tc)) return tc.GetInt32();
            if (doc.RootElement.TryGetProperty("TotalCount", out var tcP)) return tcP.GetInt32();
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get pending invoices count");
            return 0;
        }
    }

    public async Task<decimal> GetRevenueAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var url = $"api/v1/invoices/revenue?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return 0;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var data = doc.RootElement.TryGetProperty("data", out var d) ? d : doc.RootElement;
            if (data.TryGetProperty("totalCollected", out var tc)) return tc.GetDecimal();
            if (data.TryGetProperty("revenue", out var rev)) return rev.GetDecimal();
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get revenue from Billing Service");
            return 0;
        }
    }

    public async Task<IEnumerable<RevenueTrendPointDto>> GetRevenueTrendAsync(DateTime fromDate, DateTime toDate)
    {
        try
        {
            var url = $"api/v1/invoices/revenue?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return Enumerable.Empty<RevenueTrendPointDto>();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var data = doc.RootElement.TryGetProperty("data", out var d) ? d : doc.RootElement;
            
            if (data.TryGetProperty("revenueTrend", out var trend))
            {
                return JsonSerializer.Deserialize<List<RevenueTrendPointDto>>(trend.GetRawText(), new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }) ?? new List<RevenueTrendPointDto>();
            }

            return Enumerable.Empty<RevenueTrendPointDto>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get revenue trend from Billing Service");
            return Enumerable.Empty<RevenueTrendPointDto>();
        }
    }

    public async Task<(decimal TotalInvoiced, decimal TotalCollected, decimal Outstanding)> GetRevenueSummaryAsync(
        DateTime fromDate, DateTime toDate, string? customerId = null, string? serviceType = null)
    {
        try
        {
            var url = $"api/v1/invoices/revenue?fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}";
            if (!string.IsNullOrWhiteSpace(customerId)) url += $"&customerId={customerId}";
            if (!string.IsNullOrWhiteSpace(serviceType)) url += $"&serviceType={serviceType}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return (0, 0, 0);

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var data = doc.RootElement.TryGetProperty("data", out var d) ? d : doc.RootElement;

            var invoiced = data.TryGetProperty("totalInvoiced", out var ti) ? ti.GetDecimal() : 0;
            var collected = data.TryGetProperty("totalCollected", out var tc) ? tc.GetDecimal() : 0;
            var outstanding = data.TryGetProperty("outstanding", out var os) ? os.GetDecimal() : invoiced - collected;

            return (invoiced, collected, outstanding);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get revenue summary from Billing Service");
            return (0, 0, 0);
        }
    }
}
