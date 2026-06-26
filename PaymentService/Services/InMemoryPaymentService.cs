using ExternalMocks.Bank;
using PaymentService.Entities;
using Shared.Errors;
using Shared.Storage;

namespace PaymentService.Services;

public class InMemoryPaymentService : IPaymentService
{
    private readonly InMemoryStore<Payment> _payments;
    private readonly IBankClient _bankClient;

    public InMemoryPaymentService(IBankClient bankClient)
    {
        if (bankClient is null)
            throw new ArgumentNullException(nameof(bankClient));

        _payments = new InMemoryStore<Payment>();
        _bankClient = bankClient;
    }

    public Payment ProcessPayment(long clientId, long bookingId, decimal cost)
    {
        if (clientId <= 0)
            throw new ArgumentException("Client id must be positive", nameof(clientId));

        if (bookingId <= 0)
            throw new ArgumentException("Booking id must be positive", nameof(bookingId));

        if (cost < 0)
            throw new ArgumentException("Cost must not be negative", nameof(cost));

        if (HasSucceedPayment(bookingId))
            throw new PaymentFailedException($"Booking {bookingId} is already paid");

        var charged = _bankClient.TryCharge(clientId, cost);

        if (!charged)
            throw new PaymentFailedException("bank declined the charge");

        var payment = Payment.Create(_payments.NextId(), clientId, bookingId, cost);
        payment.MarkSucceed();

        return _payments.Add(payment);
    }

    public Payment GetPayment(long paymentId)
        => FindPayment(paymentId) ?? throw new PaymentNotFoundException(paymentId);

    public Payment? FindPayment(long paymentId)
        => _payments.Find(paymentId);

    public IReadOnlyCollection<Payment> GetPaymentsByClient(long clientId)
        => _payments
            .GetAll()
            .Where(payment => payment.ClientId == clientId)
            .ToList();

    public void RemovePayment(long paymentId)
    {
        if (!_payments.TryRemove(paymentId))
            throw new PaymentNotFoundException(paymentId);
    }

    private bool HasSucceedPayment(long bookingId)
        => _payments
            .GetAll()
            .Any(payment => payment.BookingId == bookingId && payment.IsSucceed);
}
