using AutoMapper;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Queries.GetOverdueInvoices;

public class GetOverdueInvoicesQueryHandler : IRequestHandler<GetOverdueInvoicesQuery, ApiResponse<IEnumerable<InvoiceDto>>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public GetOverdueInvoicesQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<InvoiceDto>>> Handle(GetOverdueInvoicesQuery request, CancellationToken cancellationToken)
    {
        var invoices = await _invoiceRepository.GetOverdueAsync();

        // Mark them as overdue in DB
        foreach (var invoice in invoices.Where(i => i.IsOverdue()))
            invoice.MarkOverdue();

        var dtos = _mapper.Map<IEnumerable<InvoiceDto>>(invoices);
        return ApiResponse<IEnumerable<InvoiceDto>>.Ok(dtos);
    }
}
