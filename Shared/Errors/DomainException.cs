namespace Shared.Errors;

public abstract class DomainException : Exception
{
    protected DomainException(string message)
        : base(message)
    {
    }

    public abstract int StatusCode { get; }
}
