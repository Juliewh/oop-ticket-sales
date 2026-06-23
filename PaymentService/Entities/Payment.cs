using Shared.Storage;

namespace PaymentService.Entities;

public class Payment : IEntity
{
    private Payment(long id, long clientId, long bookingId, decimal cost)
    {
        if (clientId <= 0)
            throw new ArgumentException("Client id must be positive", nameof(clientId));

        if (bookingId <= 0)
            throw new ArgumentException("Booking id must be positive", nameof(bookingId));

        if (cost < 0)
            throw new ArgumentException("Cost must not be negative", nameof(cost));

        Id = id;
        ClientId = clientId;
        BookingId = bookingId;
        Cost = cost;
        IsSucceed = false;
    }

    public long Id { get; }

    public long ClientId { get; }

    public long BookingId { get; }

    public decimal Cost { get; }

    public bool IsSucceed { get; private set; }

    public static Payment Create(long id, long clientId, long bookingId, decimal cost)
        => new Payment(id, clientId, bookingId, cost);

    public void MarkSucceed()
    {
        if (IsSucceed)
            throw new InvalidOperationException("Payment is already succeed");

        IsSucceed = true;
    }
}
