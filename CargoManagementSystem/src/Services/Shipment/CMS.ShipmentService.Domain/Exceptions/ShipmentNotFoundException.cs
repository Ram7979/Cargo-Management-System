namespace CMS.ShipmentService.Domain.Exceptions;

public class ShipmentNotFoundException : Exception
{
    public ShipmentNotFoundException(string message) : base(message) { }
}
