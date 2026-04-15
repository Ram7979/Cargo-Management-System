using AutoMapper;
using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetShipmentReport;

public class GetShipmentReportQueryHandler
    : IRequestHandler<GetShipmentReportQuery, PagedResponse<ShipmentReportDto>>
{
    private readonly IShipmentReadModelRepository _repository;
    private readonly IMapper _mapper;

    public GetShipmentReportQueryHandler(IShipmentReadModelRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<ShipmentReportDto>> Handle(
        GetShipmentReportQuery request,
        CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 50 : request.PageSize;
        var filter = request.Filter;

        var (items, totalCount) = await _repository.GetPagedAsync(
            page, pageSize,
            filter.Status, filter.CustomerCode,
            filter.Origin, filter.Destination,
            filter.FromDate, filter.ToDate);

        var dtos = _mapper.Map<IEnumerable<ShipmentReportDto>>(items);

        return PagedResponse<ShipmentReportDto>.Ok(dtos, page, pageSize, totalCount);
    }
}
