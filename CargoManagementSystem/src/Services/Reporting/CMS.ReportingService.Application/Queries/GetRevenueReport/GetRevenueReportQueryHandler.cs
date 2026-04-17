using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Application.Interfaces;
using CMS.ReportingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetRevenueReport;

public class GetRevenueReportQueryHandler
    : IRequestHandler<GetRevenueReportQuery, ApiResponse<RevenueReportDto>>
{
    private readonly IShipmentReadModelRepository _repository;
    private readonly IInvoiceServiceClient _invoiceClient;

    public GetRevenueReportQueryHandler(
        IShipmentReadModelRepository repository,
        IInvoiceServiceClient invoiceClient)
    {
        _repository = repository;
        _invoiceClient = invoiceClient;
    }

    public async Task<ApiResponse<RevenueReportDto>> Handle(
        GetRevenueReportQuery request,
        CancellationToken cancellationToken)
    {
        var (totalInvoiced, totalCollected, outstanding) = await _invoiceClient.GetRevenueSummaryAsync(
            request.FromDate, request.ToDate, request.CustomerId, request.ServiceType);

        // Build service-type breakdown from read model
        var allShipments = await _repository.GetAllFilteredAsync(
            fromDate: request.FromDate,
            toDate: request.ToDate,
            serviceType: request.ServiceType);

        var byServiceType = allShipments
            .GroupBy(s => string.IsNullOrWhiteSpace(s.ServiceType) ? "Unknown" : s.ServiceType)
            .Select(g => new RevenueBreakdownDto
            {
                Label = g.Key,
                TotalInvoiced = g.Sum(s => s.InvoiceAmount),
                TotalCollected = 0, // would need billing service per-type breakdown
                Outstanding = 0
            })
            .OrderByDescending(b => b.TotalInvoiced)
            .ToList();

        // Customer breakdown
        var customerStats = await _repository.GetCustomerRevenueAsync(
            request.FromDate, request.ToDate, request.CustomerId);

        var byCustomer = customerStats.Select(c => new RevenueBreakdownDto
        {
            Label = $"{c.CustomerCode} - {c.CustomerName}",
            TotalInvoiced = c.TotalInvoiced,
            TotalCollected = c.TotalPaid,
            Outstanding = c.Outstanding
        }).ToList();

        var dto = new RevenueReportDto
        {
            TotalInvoiced = totalInvoiced,
            TotalCollected = totalCollected,
            Outstanding = outstanding,
            ByServiceType = byServiceType,
            ByCustomer = byCustomer,
            FromDate = request.FromDate,
            ToDate = request.ToDate
        };

        return ApiResponse<RevenueReportDto>.Ok(dto);
    }
}
