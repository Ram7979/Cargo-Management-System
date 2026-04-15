using AutoMapper;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Domain.Interfaces;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetReceipt;

public class GetReceiptQueryHandler : IRequestHandler<GetReceiptQuery, ApiResponse<CargoReceiptDto>>
{
    private readonly ICargoReceiptRepository _receiptRepository;
    private readonly IMapper _mapper;

    public GetReceiptQueryHandler(ICargoReceiptRepository receiptRepository, IMapper mapper)
    {
        _receiptRepository = receiptRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CargoReceiptDto>> Handle(GetReceiptQuery request, CancellationToken cancellationToken)
    {
        var receipt = await _receiptRepository.GetByIdAsync(request.ReceiptId)
            ?? throw new NotFoundException("CargoReceipt", request.ReceiptId);

        var dto = _mapper.Map<CargoReceiptDto>(receipt);
        return ApiResponse<CargoReceiptDto>.Ok(dto);
    }
}
