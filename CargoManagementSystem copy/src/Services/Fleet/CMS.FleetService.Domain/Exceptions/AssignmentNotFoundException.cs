namespace CMS.FleetService.Domain.Exceptions;

public class AssignmentNotFoundException : Exception
{
    public AssignmentNotFoundException(string message) : base(message) { }
}
