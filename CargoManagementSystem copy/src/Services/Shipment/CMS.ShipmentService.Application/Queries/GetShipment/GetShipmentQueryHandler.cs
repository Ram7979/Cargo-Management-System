using AutoMapper;
using CMS.ShipmentService.Application.DTOs;
using CMS.ShipmentService.Application.Interfaces;
using CMS.ShipmentService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Queries.GetShipment;

public class GetShipmentQueryHandler : IRequestHandler<GetShipmentQuery, ApiResponse<ShipmentDto>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public GetShipmentQueryHandler(
        IShipmentRepository shipmentRepository,
        ICacheService cacheService,
        IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ShipmentDto>> Handle(GetShipmentQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"shipment:{request.ShipmentId}";
        var cached = await _cacheService.GetAsync<ShipmentDto>(cacheKey);
        if (cached is not null)
            return ApiResponse<ShipmentDto>.Ok(cached);

        var shipment = await _shipmentRepository.GetByIdAsync(request.ShipmentId)
            ?? throw new NotFoundException("Shipment", request.ShipmentId);

        var dto = _mapper.Map<ShipmentDto>(shipment);
        await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromSeconds(30));

        return ApiResponse<ShipmentDto>.Ok(dto);
    }
}
