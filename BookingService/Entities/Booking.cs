using Shared.Storage;

namespace BookingService.Entities;

public class Booking : IEntity
{
    private static readonly TimeSpan PaymentTimeout = TimeSpan.FromMinutes(30);
    private Booking(long id, long clientId, long eventId, long bookingListId, DateTime dateTime)
    {
        if (clientId <= 0)
            throw new ArgumentException("Client id must be positive", nameof(clientId));

        if (eventId <= 0)
            throw new ArgumentException("Event id must be positive", nameof(eventId));

        if (bookingListId <= 0)
            throw new ArgumentException("Booking list id must be positive", nameof(bookingListId));

        Id = id;
        ClientId = clientId;
        EventId = eventId;
        BookingListId = bookingListId;
        DateTime = dateTime;
        Status = BookingStatus.Reserved;
    }

    public long Id { get; }

    public long ClientId { get; }

    public long EventId { get; }

    public long BookingListId { get; }

    public DateTime DateTime { get; }

    public BookingStatus Status { get; private set; }

    public static Booking Create(long id, long clientId, long eventId, long bookingListId, DateTime dateTime)
        => new Booking(id, clientId, eventId, bookingListId, dateTime);

    public bool IsExpired(DateTime now)
    => Status is BookingStatus.Reserved && now - DateTime > PaymentTimeout;

    public void MarkPaid()
    {
        if (Status is not BookingStatus.Reserved)
            throw new InvalidOperationException($"Cannot pay booking in status {Status}");

        Status = BookingStatus.Paid;
    }

    public void Cancel()
    {
        if (Status is BookingStatus.Paid)
            throw new InvalidOperationException("Cannot cancel an already paid booking");

        Status = BookingStatus.Cancelled;
    }
}
