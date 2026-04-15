namespace CMS.FleetService.Domain.Exceptions;

public class InsufficientCapacityException : Exception
{
    public InsufficientCapacityException(string message) : base(message) { }
}
