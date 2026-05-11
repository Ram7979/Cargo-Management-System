namespace CMS.IdentityService.Domain.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Invalid credentials provided.") { }

    public InvalidCredentialsException(string message)
        : base(message) { }
}
