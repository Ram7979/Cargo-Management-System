namespace CMS.Shared.Exceptions;

public class UnprocessableException : Exception
{
    public UnprocessableException(string message) : base(message) { }
}
