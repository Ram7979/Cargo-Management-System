using AutoMapper;
using CMS.ShipmentService.Application.DTOs;
using CMS.ShipmentService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Queries.GetShipmentStatusHistory;

public class GetShipmentStatusHistoryQueryHandler
    : IRequestHandler<GetShipmentStatusHistoryQuery, ApiResponse<IEnumerable<ShipmentStatusHistoryDto>>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IMapper _mapper;

    public GetShipmentStatusHistoryQueryHandler(
        IShipmentRepository shipmentRepository,
        IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<ShipmentStatusHistoryDto>>> Handle(
        GetShipmentStatusHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(request.ShipmentId)
            ?? throw new NotFoundException("Shipment", request.ShipmentId);

        var history = shipment.StatusHistory
            .OrderBy(h => h.ChangedAt)
            .ToList();

        var dtos = _mapper.Map<IEnumerable<ShipmentStatusHistoryDto>>(history);
        return ApiResponse<IEnumerable<ShipmentStatusHistoryDto>>.Ok(dtos);
    }
}
