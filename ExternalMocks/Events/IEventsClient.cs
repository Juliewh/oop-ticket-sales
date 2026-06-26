namespace ExternalMocks.Events;

public interface IEventsClient
{
    bool TryReserveSeats(long eventId, IReadOnlyCollection<long> seatIds);

    void ReleaseSeats(long eventId, IReadOnlyCollection<long> seatIds);

    long? FindReservedSeat(long eventId, IReadOnlyCollection<long> seatIds);
}
