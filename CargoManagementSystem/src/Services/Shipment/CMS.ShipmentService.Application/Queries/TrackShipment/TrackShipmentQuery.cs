using CMS.ShipmentService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Queries.TrackShipment;

public class TrackShipmentQuery : IRequest<ApiResponse<TrackShipmentResponse>>
{
    public string TrackingNumber { get; }

    public TrackShipmentQuery(string trackingNumber)
    {
        TrackingNumber = trackingNumber;
    }
}
