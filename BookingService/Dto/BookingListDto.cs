namespace BookingService.Dto;

public record BookingListDto
{
    public long Id { get; init; }

    public long BookingId { get; init; }

    public IReadOnlyCollection<long> SeatIds { get; init; } = new List<long>();
}
