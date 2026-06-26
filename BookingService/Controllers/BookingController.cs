using BookingService.Dto;
using BookingService.Mapping;
using BookingService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[ApiController]
[Route("booking")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        if (bookingService is null)
            throw new ArgumentNullException(nameof(bookingService));

        _bookingService = bookingService;
    }

    [HttpPost("add")]
    public BookingDto Add(CreateBookingRequest request)
    {
        var booking = _bookingService.CreateBooking(request.ClientId, request.EventId, request.SeatIds);

        return BookingMapping.ToDto.Compile().Invoke(booking);
    }
    
    [HttpPost("pay/{id}")]
    public BookingDto Pay(long id, [FromQuery] decimal cost)
    {
        var booking = _bookingService.PayBooking(id, cost);

        return BookingMapping.ToDto.Compile().Invoke(booking);
    }

    [HttpGet("getAll")]
    public IReadOnlyCollection<BookingDto> GetAll()
        => _bookingService
            .GetAllBookings()
            .Select(BookingMapping.ToDto.Compile())
            .ToList();

    [HttpGet("get/{id}")]
    public BookingDto Get(long id)
    {
        var booking = _bookingService.GetBooking(id);

        return BookingMapping.ToDto.Compile().Invoke(booking);
    }

    [HttpGet("getAllByClientId/{clientId}")]
    public IReadOnlyCollection<BookingDto> GetAllByClientId(long clientId)
        => _bookingService
            .GetBookingsByClient(clientId)
            .Select(BookingMapping.ToDto.Compile())
            .ToList();

    [HttpDelete("{id}")]
    public void Delete(long id)
        => _bookingService.CancelBooking(id);
}
