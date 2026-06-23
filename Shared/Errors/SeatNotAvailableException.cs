namespace Shared.Errors;

public class SeatNotAvailableException : DomainException
{
    public SeatNotAvailableException(long seatId)
        : base($"Seat with id {seatId} is not available")
    {
    }

    public override int StatusCode => 400;
}
