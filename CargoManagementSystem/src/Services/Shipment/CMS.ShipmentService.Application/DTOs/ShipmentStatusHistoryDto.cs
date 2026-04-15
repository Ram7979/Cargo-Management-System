namespace CMS.ShipmentService.Application.DTOs;

public class ShipmentStatusHistoryDto
{
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public string ActorId { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public GpsCoordinateDto? Location { get; set; }
}
