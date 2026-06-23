using Shared.Storage;

namespace BookingService.Entities;

public class BookingList : IEntity
{
    private readonly List<long> _seatIds;

    private BookingList(long id, IReadOnlyCollection<long> seatIds)
    {
        if (seatIds is null)
            throw new ArgumentNullException(nameof(seatIds));

        if (seatIds.Count == 0)
            throw new ArgumentException("Booking list must contain at least one seat", nameof(seatIds));

        if (seatIds.Distinct().Count() != seatIds.Count)
            throw new ArgumentException("Booking list must not contain duplicate seats", nameof(seatIds));

        _seatIds = new List<long>();

        Id = id;

        _seatIds.AddRange(seatIds);
    }

    public long Id { get; }

    public IReadOnlyCollection<long> SeatIds => _seatIds;

    public static BookingList Create(long id, IReadOnlyCollection<long> seatIds)
        => new BookingList(id, seatIds);
}
