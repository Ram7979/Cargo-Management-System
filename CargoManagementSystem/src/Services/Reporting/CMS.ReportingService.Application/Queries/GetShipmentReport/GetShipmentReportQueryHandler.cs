using AutoMapper;
using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetShipmentReport;

public class GetShipmentReportQueryHandler
    : IRequestHandler<GetShipmentReportQuery, ApiResponse<PagedResponse<ShipmentReportDto>>>
{
    private readonly IShipmentReadModelRepository _repository;
    private readonly IMapper _mapper;

    public GetShipmentReportQueryHandler(IShipmentReadModelRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResponse<ShipmentReportDto>>> Handle(
        GetShipmentReportQuery request,
        CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 50 : request.PageSize;
        var f = request.Filter;

        var (items, totalCount) = await _repository.GetPagedAsync(
            page, pageSize,
            f.Status, f.CustomerCode,
            f.Origin, f.Destination,
            f.FromDate, f.ToDate,
            f.DriverId, f.VehicleId,
            f.ServiceType, f.SortBy, f.SortDir);

        var dtos = _mapper.Map<IEnumerable<ShipmentReportDto>>(items);
        var paged = PagedResponse<ShipmentReportDto>.Ok(dtos, page, pageSize, totalCount);
        return ApiResponse<PagedResponse<ShipmentReportDto>>.Ok(paged);
    }
}
