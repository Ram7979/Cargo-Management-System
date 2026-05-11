using AutoMapper;
using CMS.ShipmentService.Application.DTOs;
using CMS.ShipmentService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Queries.GetShipments;

public class GetShipmentsQueryHandler : IRequestHandler<GetShipmentsQuery, PagedResponse<ShipmentDto>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IMapper _mapper;

    public GetShipmentsQueryHandler(IShipmentRepository shipmentRepository, IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<ShipmentDto>> Handle(GetShipmentsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _shipmentRepository.GetPagedAsync(
            request.Page, request.PageSize,
            request.Status, request.CustomerId,
            request.DateFrom, request.DateTo,
            request.ServiceType, request.TrackingNumberSearch,
            request.SortBy, request.SortDir);

        var dtos = _mapper.Map<IEnumerable<ShipmentDto>>(items);
        return PagedResponse<ShipmentDto>.Ok(dtos, request.Page, request.PageSize, totalCount);
    }
}
