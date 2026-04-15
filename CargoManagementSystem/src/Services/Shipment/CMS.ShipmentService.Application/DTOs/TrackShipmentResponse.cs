namespace CMS.ShipmentService.Application.DTOs;

public class TrackShipmentResponse
{
    public ShipmentDto Shipment { get; set; } = null!;
    public IEnumerable<ShipmentStatusHistoryDto> Timeline { get; set; } = Enumerable.Empty<ShipmentStatusHistoryDto>();
}
