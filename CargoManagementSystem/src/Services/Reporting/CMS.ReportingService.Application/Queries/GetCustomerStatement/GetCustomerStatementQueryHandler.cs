using AutoMapper;
using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Application.Interfaces;
using CMS.ReportingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetCustomerStatement;

public class GetCustomerStatementQueryHandler
    : IRequestHandler<GetCustomerStatementQuery, ApiResponse<CustomerStatementDto>>
{
    private readonly IShipmentReadModelRepository _repository;
    private readonly IInvoiceServiceClient _invoiceClient;
    private readonly IMapper _mapper;

    public GetCustomerStatementQueryHandler(
        IShipmentReadModelRepository repository,
        IInvoiceServiceClient invoiceClient,
        IMapper mapper)
    {
        _repository = repository;
        _invoiceClient = invoiceClient;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CustomerStatementDto>> Handle(
        GetCustomerStatementQuery request,
        CancellationToken cancellationToken)
    {
        var shipmentsTask = _repository.GetAllFilteredAsync(
            fromDate: request.FromDate,
            toDate: request.ToDate);

        var revenueTask = _invoiceClient.GetRevenueSummaryAsync(
            request.FromDate, request.ToDate, request.CustomerId);

        await Task.WhenAll(shipmentsTask, revenueTask);

        var allShipments = (await shipmentsTask)
            .Where(s => s.CustomerCode == request.CustomerId || s.Id.ToString() == request.CustomerId)
            .ToList();

        var (totalInvoiced, totalPaid, outstanding) = await revenueTask;

        var shipmentDtos = _mapper.Map<IEnumerable<ShipmentReportDto>>(allShipments);

        var customerName = allShipments.FirstOrDefault()?.CustomerName ?? string.Empty;
        var customerCode = allShipments.FirstOrDefault()?.CustomerCode ?? request.CustomerId;

        var dto = new CustomerStatementDto
        {
            CustomerId = request.CustomerId,
            CustomerCode = customerCode,
            CustomerName = customerName,
            TotalInvoiced = totalInvoiced,
            TotalPaid = totalPaid,
            OutstandingBalance = outstanding,
            TotalShipments = allShipments.Count,
            Shipments = shipmentDtos,
            FromDate = request.FromDate,
            ToDate = request.ToDate
        };

        return ApiResponse<CustomerStatementDto>.Ok(dto);
    }
}
