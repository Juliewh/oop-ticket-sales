namespace PaymentService.Dto;

public record PaymentDto
{
    public long Id { get; init; }

    public decimal Cost { get; init; }

    public bool IsSucceed { get; init; }

    public long TicketId { get; init; }

    public long ClientId { get; init; }
}
