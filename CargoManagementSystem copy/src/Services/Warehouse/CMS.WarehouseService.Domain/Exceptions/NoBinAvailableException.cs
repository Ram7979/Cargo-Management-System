namespace CMS.WarehouseService.Domain.Exceptions;

public class NoBinAvailableException : Exception
{
    public NoBinAvailableException(string message) : base(message) { }
}
