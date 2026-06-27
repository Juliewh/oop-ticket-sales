namespace BookingService.Services;

public interface IPaymentGateway
{
    bool TryPay(long clientId, long bookingId, long eventId, IReadOnlyCollection<long> seatIds, decimal cost);
}
