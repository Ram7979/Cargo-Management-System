using AutoMapper;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Queries.GetInvoicePayments;

public class GetInvoicePaymentsQueryHandler : IRequestHandler<GetInvoicePaymentsQuery, ApiResponse<IEnumerable<PaymentDto>>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetInvoicePaymentsQueryHandler(
        IInvoiceRepository invoiceRepository,
        IPaymentRepository paymentRepository,
        IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<PaymentDto>>> Handle(GetInvoicePaymentsQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId)
            ?? throw new NotFoundException("Invoice", request.InvoiceId);

        var payments = await _paymentRepository.GetByInvoiceIdAsync(request.InvoiceId);
        var dtos = _mapper.Map<IEnumerable<PaymentDto>>(payments);
        return ApiResponse<IEnumerable<PaymentDto>>.Ok(dtos);
    }
}
