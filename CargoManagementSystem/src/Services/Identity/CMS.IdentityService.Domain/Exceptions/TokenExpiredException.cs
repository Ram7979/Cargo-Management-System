namespace CMS.IdentityService.Domain.Exceptions;

public class TokenExpiredException : Exception
{
    public TokenExpiredException()
        : base("The token has expired.") { }

    public TokenExpiredException(string message)
        : base(message) { }
}
