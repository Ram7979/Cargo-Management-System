namespace CMS.CustomerService.Domain.Exceptions;

public class CustomerNotFoundException : Exception
{
    public CustomerNotFoundException(string message) : base(message) { }

    public CustomerNotFoundException(Guid id)
        : base($"Customer with ID '{id}' was not found.") { }
}
