namespace PaymentService.Dto;

public record CreatePaymentRequest
{
    public long ClientId { get; init; }

    public long BookingId { get; init; }

    public long EventId { get; init; }

    public IReadOnlyCollection<long> SeatIds { get; init; } = new List<long>();

    public decimal Cost { get; init; }
}
