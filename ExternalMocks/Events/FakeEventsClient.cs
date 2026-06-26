using Shared.Errors;

namespace ExternalMocks.Events;

public class FakeEventsClient : IEventsClient
{
    private readonly Dictionary<long, DateTime> _events;
    private readonly HashSet<(long EventId, long SeatId)> _reservedSeats;

    public FakeEventsClient()
    {
        _events = new Dictionary<long, DateTime>
        {
            [1] = DateTime.UtcNow.AddDays(30),
        };
        _reservedSeats = new HashSet<(long EventId, long SeatId)>();
    }

    public void AddEvent(long eventId, DateTime startsAt)
    {
        if (eventId <= 0)
            throw new ArgumentException("Event id must be positive", nameof(eventId));

        _events[eventId] = startsAt;
    }

    public void RemoveEvent(long eventId)
    {
        if (eventId <= 0)
            throw new ArgumentException("Event id must be positive", nameof(eventId));

        _events.Remove(eventId);
    }

    public bool TryReserveSeats(long eventId, IReadOnlyCollection<long> seatIds)
    {
        EnsureEventIsAvailable(eventId);

        if (seatIds is null || seatIds.Count == 0)
            throw new ArgumentException("Seat ids are required", nameof(seatIds));

        if (seatIds.Any(seatId => _reservedSeats.Contains((eventId, seatId))))
            return false;

        foreach (var seatId in seatIds)
            _reservedSeats.Add((eventId, seatId));

        return true;
    }

    public long? FindReservedSeat(long eventId, IReadOnlyCollection<long> seatIds)
    {
        if (seatIds is null)
            throw new ArgumentNullException(nameof(seatIds));

        foreach (var seatId in seatIds)
        {
            if (_reservedSeats.Contains((eventId, seatId)))
                return seatId;
        }

        return null;
    }

    public void ReleaseSeats(long eventId, IReadOnlyCollection<long> seatIds)
    {
        if (seatIds is null)
            throw new ArgumentNullException(nameof(seatIds));

        foreach (var seatId in seatIds)
            _reservedSeats.Remove((eventId, seatId));
    }

    private void EnsureEventIsAvailable(long eventId)
    {
        if (!_events.TryGetValue(eventId, out var startsAt))
            throw new EventNotFoundException(eventId);

        if (startsAt <= DateTime.UtcNow)
            throw new EventUnavailableException(eventId);
    }
}
