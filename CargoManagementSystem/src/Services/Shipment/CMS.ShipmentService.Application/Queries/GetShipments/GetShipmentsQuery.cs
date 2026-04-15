using CMS.ShipmentService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Queries.GetShipments;

public class GetShipmentsQuery : IRequest<PagedResponse<ShipmentDto>>
{
    public int Page { get; }
    public int PageSize { get; }
    public string? Status { get; }
    public Guid? CustomerId { get; }
    public DateTime? DateFrom { get; }
    public DateTime? DateTo { get; }
    public string? ServiceType { get; }
    public string? TrackingNumberSearch { get; }
    public string? SortBy { get; }
    public string? SortDir { get; }

    public GetShipmentsQuery(
        int page, int pageSize,
        string? status = null,
        Guid? customerId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        string? serviceType = null,
        string? trackingNumberSearch = null,
        string? sortBy = null,
        string? sortDir = null)
    {
        Page = page;
        PageSize = pageSize;
        Status = status;
        CustomerId = customerId;
        DateFrom = dateFrom;
        DateTo = dateTo;
        ServiceType = serviceType;
        TrackingNumberSearch = trackingNumberSearch;
        SortBy = sortBy;
        SortDir = sortDir;
    }
}
