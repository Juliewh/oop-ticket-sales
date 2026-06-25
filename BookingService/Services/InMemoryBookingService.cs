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
        if (clientId <= 0)
            throw new ArgumentException("Client id must be positive", nameof(clientId));

        if (eventId <= 0)
            throw new ArgumentException("Event id must be positive", nameof(eventId));

        if (seatIds is null || seatIds.Count == 0)
            throw new ArgumentException("At least one seat is required", nameof(seatIds));

        if (seatIds.Distinct().Count() != seatIds.Count)
            throw new ArgumentException("Seats must not be duplicated", nameof(seatIds));

        var reserved = _eventsClient.TryReserveSeats(eventId, seatIds);

         if (!reserved)
        {
            var takenSeatId = _eventsClient.FindReservedSeat(seatIds);

            throw new SeatNotAvailableException(takenSeatId ?? seatIds.First());
        }

        return CreateBookingForReservedSeats(clientId, eventId, seatIds);
    }

    public Booking GetBooking(long bookingId)
    {
        var booking = FindBooking(bookingId) ?? throw new BookingNotFoundException(bookingId);

        if (booking.IsExpired(DateTime.UtcNow))
            CancelExpiredBooking(booking);

        return booking;
    }
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

    private Booking CreateBookingForReservedSeats(long clientId, long eventId, IReadOnlyCollection<long> seatIds)
    {
        try
        {
            var bookingList = BookingList.Create(_bookingLists.NextId(), seatIds);
            _bookingLists.Add(bookingList);

            var booking = Booking.Create(_bookings.NextId(), clientId, eventId, bookingList.Id, DateTime.UtcNow);

            return _bookings.Add(booking);
        }
        catch
        {
            // Release the seats so a failed booking does not leak reservations (rule 12).
            _eventsClient.ReleaseSeats(eventId, seatIds);

            throw;
        }
    }

    private void CancelExpiredBooking(Booking booking)
    {
        var bookingList = _bookingLists.Find(booking.BookingListId);

        booking.Cancel();

        if (bookingList is not null)
            _eventsClient.ReleaseSeats(booking.EventId, bookingList.SeatIds);
    }
}