namespace CMS.FleetService.Domain.Exceptions;

public class VehicleNotFoundException : Exception
{
    public VehicleNotFoundException(string message) : base(message) { }
}
