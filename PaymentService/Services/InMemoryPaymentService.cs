using ExternalMocks.Bank;
using ExternalMocks.Tickets;
using PaymentService.Entities;
using Shared.Errors;
using Shared.Storage;

namespace PaymentService.Services;

public class InMemoryPaymentService : IPaymentService
{
    private readonly InMemoryStore<Payment> _payments;
    private readonly IBankClient _bankClient;
    private readonly ITicketsClient _ticketsClient;

    public InMemoryPaymentService(IBankClient bankClient, ITicketsClient ticketsClient)
    {
        if (bankClient is null)
            throw new ArgumentNullException(nameof(bankClient));

        if (ticketsClient is null)
            throw new ArgumentNullException(nameof(ticketsClient));

        _payments = new InMemoryStore<Payment>();
        _bankClient = bankClient;
        _ticketsClient = ticketsClient;
    }

    public Payment ProcessPayment(long clientId, long bookingId, long eventId, IReadOnlyCollection<long> seatIds, decimal cost)
    {
        if (clientId <= 0)
            throw new ArgumentException("Client id must be positive", nameof(clientId));

        if (bookingId <= 0)
            throw new ArgumentException("Booking id must be positive", nameof(bookingId));

        if (eventId <= 0)
            throw new ArgumentException("Event id must be positive", nameof(eventId));

        if (seatIds is null || seatIds.Count == 0)
            throw new ArgumentException("At least one seat is required", nameof(seatIds));

        if (cost < 0)
            throw new ArgumentException("Cost must not be negative", nameof(cost));

        if (HasSucceedPayment(bookingId))
            throw new PaymentFailedException($"Booking {bookingId} is already paid");

        var charged = _bankClient.TryCharge(clientId, cost);

        if (!charged)
            throw new PaymentFailedException("bank declined the charge");

        var ticketIds = seatIds
            .Select(seatId => _ticketsClient.IssueTicket(clientId, eventId, seatId))
            .ToList();

        var payment = Payment.Create(_payments.NextId(), clientId, bookingId, cost, ticketIds);
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
