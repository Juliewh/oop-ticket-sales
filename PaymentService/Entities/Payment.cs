using Shared.Storage;

namespace PaymentService.Entities;

public class Payment : IEntity
{
    private Payment(long id, long clientId, long bookingId, decimal cost, IReadOnlyCollection<long> ticketIds)
    {
        if (clientId <= 0)
            throw new ArgumentException("Client id must be positive", nameof(clientId));

        if (bookingId <= 0)
            throw new ArgumentException("Booking id must be positive", nameof(bookingId));

        if (cost < 0)
            throw new ArgumentException("Cost must not be negative", nameof(cost));

        if (ticketIds is null || ticketIds.Count == 0)
            throw new ArgumentException("At least one ticket id is required", nameof(ticketIds));

        Id = id;
        ClientId = clientId;
        BookingId = bookingId;
        Cost = cost;
        TicketIds = ticketIds.ToList();
        IsSucceed = false;
    }

    public long Id { get; }

    public long ClientId { get; }

    public long BookingId { get; }

    public decimal Cost { get; }

    public IReadOnlyCollection<long> TicketIds { get; }

    public bool IsSucceed { get; private set; }

    public static Payment Create(long id, long clientId, long bookingId, decimal cost, IReadOnlyCollection<long> ticketIds)
        => new Payment(id, clientId, bookingId, cost, ticketIds);

    public void MarkSucceed()
    {
        if (IsSucceed)
            throw new InvalidOperationException("Payment is already succeed");

        IsSucceed = true;
    }
}
