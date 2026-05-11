namespace CMS.FleetService.Domain.Exceptions;

public class DriverNotFoundException : Exception
{
    public DriverNotFoundException(string message) : base(message) { }
}
