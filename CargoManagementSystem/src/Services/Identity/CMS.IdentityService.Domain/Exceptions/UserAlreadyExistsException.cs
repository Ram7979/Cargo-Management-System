namespace CMS.IdentityService.Domain.Exceptions;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException()
        : base("A user with this email address already exists.") { }

    public UserAlreadyExistsException(string email)
        : base($"A user with email '{email}' already exists.") { }
}
