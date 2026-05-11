using AutoMapper;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Queries.GetInvoices;

public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, PagedResponse<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public GetInvoicesQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _invoiceRepository.GetPagedAsync(
            request.Page, request.PageSize,
            request.CustomerId, request.Status,
            request.DateFrom, request.DateTo, request.ShipmentId);

        var dtos = _mapper.Map<IEnumerable<InvoiceDto>>(items);
        return PagedResponse<InvoiceDto>.Ok(dtos, request.Page, request.PageSize, totalCount);
    }
}
