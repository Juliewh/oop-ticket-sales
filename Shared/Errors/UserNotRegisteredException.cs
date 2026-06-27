namespace Shared.Errors;

public class UserNotRegisteredException : DomainException
{
    public UserNotRegisteredException(long clientId)
        : base($"Client with id {clientId} is not registered")
    {
    }

    public override int StatusCode => 401;
}
