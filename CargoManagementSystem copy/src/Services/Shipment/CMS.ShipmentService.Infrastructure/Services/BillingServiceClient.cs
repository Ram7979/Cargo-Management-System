using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CMS.ShipmentService.Application.Interfaces;
using CMS.Shared.Responses;

namespace CMS.ShipmentService.Infrastructure.Services;

public class BillingServiceClient : IBillingServiceClient
{
    private readonly HttpClient _httpClient;

    public BillingServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> GenerateInvoiceAsync(GenerateInvoiceInternalRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/invoices/internal/generate", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
