namespace BookingService.Services;

public interface IPaymentGateway
{
    bool TryPay(long clientId, long bookingId, decimal cost);
}