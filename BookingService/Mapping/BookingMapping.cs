using System.Linq.Expressions;
using BookingService.Dto;
using BookingService.Entities;

namespace BookingService.Mapping;

public static class BookingMapping
{
    public static Expression<Func<Booking, BookingDto>> ToDto => booking => new BookingDto
    {
        Id = booking.Id,
        ClientId = booking.ClientId,
        DateTime = booking.DateTime.ToString("O"),
        EventId = booking.EventId,
        BookingListId = booking.BookingListId,
    };
}
