using BookingService.Entities;
using ExternalMocks.Events;
using Shared.Errors;
using Shared.Storage;

namespace BookingService.Services;

public class InMemoryBookingService : IBookingService
{
    private readonly InMemoryStore<Booking> _bookings;
    private readonly InMemoryStore<BookingList> _bookingLists;
    private readonly IEventsClient _eventsClient;

    public InMemoryBookingService(IEventsClient eventsClient)
    {
        if (eventsClient is null)
            throw new ArgumentNullException(nameof(eventsClient));

        _bookings = new InMemoryStore<Booking>();
        _bookingLists = new InMemoryStore<BookingList>();
        _eventsClient = eventsClient;
    }

    public Booking CreateBooking(long clientId, long eventId, IReadOnlyCollection<long> seatIds)
    {
        if (seatIds is null || seatIds.Count == 0)
            throw new ArgumentException("At least one seat is required", nameof(seatIds));

        // TODO (rule 12): reserve all seats atomically. If the mock fails on any seat,
        // release everything already reserved and throw SeatNotAvailableException.
        var reserved = _eventsClient.TryReserveSeats(eventId, seatIds);

        if (!reserved)
            throw new SeatNotAvailableException(seatIds.First());

        var bookingList = BookingList.Create(_bookingLists.NextId(), seatIds);
        _bookingLists.Add(bookingList);

        var booking = Booking.Create(_bookings.NextId(), clientId, eventId, bookingList.Id, DateTime.UtcNow);

        return _bookings.Add(booking);
    }

    public Booking GetBooking(long bookingId)
        => FindBooking(bookingId) ?? throw new BookingNotFoundException(bookingId);

    public Booking? FindBooking(long bookingId)
        => _bookings.Find(bookingId);

    public IReadOnlyCollection<Booking> GetAllBookings()
        => _bookings.GetAll();

    public IReadOnlyCollection<Booking> GetBookingsByClient(long clientId)
        => _bookings
            .GetAll()
            .Where(booking => booking.ClientId == clientId)
            .ToList();

    public void CancelBooking(long bookingId)
    {
        var booking = GetBooking(bookingId);

        booking.Cancel();

        // TODO (rule 11 / rule 8): release the seats of this booking back to the Events mock.
    }
}
