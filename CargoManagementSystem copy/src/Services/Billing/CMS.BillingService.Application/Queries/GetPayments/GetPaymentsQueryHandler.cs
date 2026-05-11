using AutoMapper;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Queries.GetPayments;

public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, PagedResponse<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentsQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<PaymentDto>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _paymentRepository.GetPagedAsync(
            request.Page, request.PageSize,
            request.InvoiceId, request.Method, request.DateFrom, request.DateTo);

        var dtos = _mapper.Map<IEnumerable<PaymentDto>>(items);
        return PagedResponse<PaymentDto>.Ok(dtos, request.Page, request.PageSize, totalCount);
    }
}
