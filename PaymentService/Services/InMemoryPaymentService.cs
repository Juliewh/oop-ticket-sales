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
        // TODO (rule 13): reject if this booking already has a succeed payment.
        var payment = Payment.Create(_payments.NextId(), clientId, bookingId, cost);

        // TODO (rule 3): treat this as a transaction. On bank failure throw
        // PaymentFailedException so the booking gets cancelled upstream (rule 7).
        var charged = _bankClient.TryCharge(clientId, cost);

        if (!charged)
            throw new PaymentFailedException("bank declined the charge");

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
}
