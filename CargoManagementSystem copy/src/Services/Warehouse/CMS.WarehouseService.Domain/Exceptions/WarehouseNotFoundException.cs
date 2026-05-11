namespace CMS.WarehouseService.Domain.Exceptions;

public class WarehouseNotFoundException : Exception
{
    public WarehouseNotFoundException(string message) : base(message) { }
}
