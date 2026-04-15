using AutoMapper;
using CMS.ShipmentService.Application.DTOs;
using CMS.ShipmentService.Application.Interfaces;
using CMS.ShipmentService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Queries.TrackShipment;

public class TrackShipmentQueryHandler : IRequestHandler<TrackShipmentQuery, ApiResponse<TrackShipmentResponse>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public TrackShipmentQueryHandler(
        IShipmentRepository shipmentRepository,
        ICacheService cacheService,
        IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<TrackShipmentResponse>> Handle(TrackShipmentQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"shipment:track:{request.TrackingNumber}";
        var cached = await _cacheService.GetAsync<TrackShipmentResponse>(cacheKey);
        if (cached is not null)
            return ApiResponse<TrackShipmentResponse>.Ok(cached);

        var shipment = await _shipmentRepository.GetByTrackingNumberAsync(request.TrackingNumber)
            ?? throw new NotFoundException($"Shipment with tracking number '{request.TrackingNumber}' was not found.");

        var response = new TrackShipmentResponse
        {
            Shipment = _mapper.Map<ShipmentDto>(shipment),
            Timeline = _mapper.Map<IEnumerable<ShipmentStatusHistoryDto>>(shipment.StatusHistory)
        };

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromSeconds(30));

        return ApiResponse<TrackShipmentResponse>.Ok(response);
    }
}
