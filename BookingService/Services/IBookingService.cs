using BookingService.Entities;

namespace BookingService.Services;

public interface IBookingService
{
    Booking CreateBooking(long clientId, long eventId, IReadOnlyCollection<long> seatIds);

    Booking GetBooking(long bookingId);

    Booking? FindBooking(long bookingId);

    IReadOnlyCollection<Booking> GetAllBookings();

    IReadOnlyCollection<Booking> GetBookingsByClient(long clientId);

    void CancelBooking(long bookingId);
}
