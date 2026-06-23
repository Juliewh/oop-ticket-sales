namespace BookingService.Dto;

public record CreateBookingRequest
{
    public long ClientId { get; init; }

    public long EventId { get; init; }

    public IReadOnlyCollection<long> SeatIds { get; init; } = new List<long>();
}
