namespace PaymentService.Dto;

public record CreatePaymentRequest
{
    public long ClientId { get; init; }

    public long BookingId { get; init; }

    public decimal Cost { get; init; }
}
