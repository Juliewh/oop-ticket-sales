namespace ExternalMocks.Events;

// Fake Events dependency: keeps reserved seats in memory so Booking can be tested
// without a real Events service. Replace with a real gRPC/HTTP client later.
public class FakeEventsClient : IEventsClient
{
    private readonly HashSet<long> _reservedSeatIds;

    public FakeEventsClient()
    {
        _reservedSeatIds = new HashSet<long>();
    }

    public bool TryReserveSeats(long eventId, IReadOnlyCollection<long> seatIds)
    {
        if (seatIds is null || seatIds.Count == 0)
            throw new ArgumentException("Seat ids are required", nameof(seatIds));

        if (seatIds.Any(seatId => _reservedSeatIds.Contains(seatId)))
            return false;

        foreach (var seatId in seatIds)
            _reservedSeatIds.Add(seatId);

        return true;
    }

    public long? FindReservedSeat(IReadOnlyCollection<long> seatIds)
{
    if (seatIds is null)
        throw new ArgumentNullException(nameof(seatIds));

    foreach (var seatId in seatIds)
    {
        if (_reservedSeatIds.Contains(seatId))
            return seatId;
    }

    return null;
}

    public void ReleaseSeats(long eventId, IReadOnlyCollection<long> seatIds)
    {
        if (seatIds is null)
            throw new ArgumentNullException(nameof(seatIds));

        foreach (var seatId in seatIds)
            _reservedSeatIds.Remove(seatId);
    }
}
