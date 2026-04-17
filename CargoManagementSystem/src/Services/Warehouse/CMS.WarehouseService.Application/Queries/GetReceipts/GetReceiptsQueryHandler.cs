using AutoMapper;
using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Domain.Interfaces;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetReceipts;

public class GetReceiptsQueryHandler : IRequestHandler<GetReceiptsQuery, ApiResponse<PagedResponse<CargoReceiptDto>>>
{
    private readonly ICargoReceiptRepository _receiptRepository;
    private readonly IMapper _mapper;

    public GetReceiptsQueryHandler(ICargoReceiptRepository receiptRepository, IMapper mapper)
    {
        _receiptRepository = receiptRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResponse<CargoReceiptDto>>> Handle(GetReceiptsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _receiptRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.WarehouseId,
            request.ShipmentId,
            request.HasDamage,
            request.DateFrom,
            request.DateTo);

        var dtos = _mapper.Map<IEnumerable<CargoReceiptDto>>(items);
        var paged = PagedResponse<CargoReceiptDto>.Ok(dtos, request.Page, request.PageSize, totalCount);
        return ApiResponse<PagedResponse<CargoReceiptDto>>.Ok(paged);
    }
}
