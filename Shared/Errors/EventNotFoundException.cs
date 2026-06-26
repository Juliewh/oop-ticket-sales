namespace Shared.Errors;

public class EventNotFoundException : DomainException
{
    public EventNotFoundException(long eventId)
        : base($"Event with id {eventId} is not found")
    {
    }

    public override int StatusCode => 404;
}
