using AutoMapper;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Commands.VoidInvoice;

public class VoidInvoiceCommandHandler : IRequestHandler<VoidInvoiceCommand, ApiResponse<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public VoidInvoiceCommandHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<InvoiceDto>> Handle(VoidInvoiceCommand command, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(command.InvoiceId)
            ?? throw new NotFoundException("Invoice", command.InvoiceId);

        try
        {
            invoice.Void(command.Request.Reason);
        }
        catch (InvalidOperationException ex)
        {
            throw new UnprocessableException(ex.Message);
        }

        await _invoiceRepository.UpdateAsync(invoice);

        var dto = _mapper.Map<InvoiceDto>(invoice);
        return ApiResponse<InvoiceDto>.Ok(dto, "Invoice voided successfully.");
    }
}
