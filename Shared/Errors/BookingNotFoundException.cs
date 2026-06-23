namespace Shared.Errors;

public class BookingNotFoundException : DomainException
{
    public BookingNotFoundException(long bookingId)
        : base($"Booking with id {bookingId} is not found")
    {
    }

    public override int StatusCode => 404;
}
