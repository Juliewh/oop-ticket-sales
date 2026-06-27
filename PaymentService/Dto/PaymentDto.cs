namespace PaymentService.Dto;

public record PaymentDto
{
    public long Id { get; init; }

    public decimal Cost { get; init; }

    public bool IsSucceed { get; init; }

    public IReadOnlyCollection<long> TicketIds { get; init; } = new List<long>();

    public long ClientId { get; init; }
}
