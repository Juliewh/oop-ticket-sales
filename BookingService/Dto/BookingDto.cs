namespace BookingService.Dto;

public record BookingDto
{
    public long Id { get; init; }

    public long ClientId { get; init; }

    public string DateTime { get; init; } = string.Empty;

    public long EventId { get; init; }

    public long BookingListId { get; init; }
}
