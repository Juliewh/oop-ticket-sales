using PaymentService.Entities;

namespace PaymentService.Services;

public interface IPaymentService
{
    Payment ProcessPayment(long clientId, long bookingId, long eventId, IReadOnlyCollection<long> seatIds, decimal cost);

    Payment GetPayment(long paymentId);

    Payment? FindPayment(long paymentId);

    IReadOnlyCollection<Payment> GetPaymentsByClient(long clientId);

    void RemovePayment(long paymentId);
}
