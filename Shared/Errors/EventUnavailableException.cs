namespace Shared.Errors;

public class EventUnavailableException : DomainException
{
    public EventUnavailableException(long eventId)
        : base($"Event with id {eventId} is unavailable")
    {
    }

    public override int StatusCode => 400;
}
