namespace CMS.FleetService.Domain.Exceptions;

public class DriverNotAvailableException : Exception
{
    public DriverNotAvailableException(string message) : base(message) { }
}
