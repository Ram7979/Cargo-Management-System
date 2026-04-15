namespace CMS.CustomerService.Domain.Exceptions;

public class CustomerAlreadyExistsException : Exception
{
    public CustomerAlreadyExistsException(string message) : base(message) { }

    public CustomerAlreadyExistsException(string email, bool _)
        : base($"A customer with email '{email}' already exists.") { }
}
