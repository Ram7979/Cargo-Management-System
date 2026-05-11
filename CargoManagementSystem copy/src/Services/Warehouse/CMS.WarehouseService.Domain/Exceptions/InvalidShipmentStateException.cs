namespace CMS.WarehouseService.Domain.Exceptions;

public class InvalidShipmentStateException : Exception
{
    public InvalidShipmentStateException(string message) : base(message) { }
}
