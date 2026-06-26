using BookingService.Entities;
using BookingService.Services;
using ExternalMocks.Bank;
using ExternalMocks.Events;
using PaymentService.Services;
using Shared.Errors;
using Xunit;

namespace BookingService.Tests;

public class BookingScenarioTests
{
    private const long ClientId = 1;
    private const long EventId = 1;
    private const decimal Cost = 100;

    private static InMemoryBookingService CreateService(BankOutcome bankOutcome)
    {
        var eventsClient = new FakeEventsClient();
        var bankClient = new FakeBankClient(bankOutcome);
        var paymentService = new InMemoryPaymentService(bankClient);
        var paymentGateway = new PaymentServiceGateway(paymentService);

        return new InMemoryBookingService(eventsClient, paymentGateway);
    }

    [Fact]
    public void PayBooking_WithWorkingBank_MarksBookingPaid()
    {
        var service = CreateService(BankOutcome.Success);
        var booking = service.CreateBooking(ClientId, EventId, new List<long> { 1, 2 });

        var paid = service.PayBooking(booking.Id, Cost);

        Assert.Equal(BookingStatus.Paid, paid.Status);
    }

    [Fact]
    public void PayBooking_WhenBankDeclines_CancelsBookingAndReleasesSeats()
    {
        var service = CreateService(BankOutcome.Decline);
        var booking = service.CreateBooking(ClientId, EventId, new List<long> { 1, 2 });

        Assert.Throws<PaymentFailedException>(() => service.PayBooking(booking.Id, Cost));
        Assert.Equal(BookingStatus.Cancelled, service.FindBooking(booking.Id)!.Status);

        var rebooking = service.CreateBooking(ClientId, EventId, new List<long> { 1, 2 });
        Assert.Equal(BookingStatus.Reserved, rebooking.Status);
    }

    [Fact]
    public void CreateBooking_WithAlreadyTakenSeat_Throws()
    {
        var service = CreateService(BankOutcome.Success);
        service.CreateBooking(ClientId, EventId, new List<long> { 1, 2 });

        Assert.Throws<SeatNotAvailableException>(
            () => service.CreateBooking(ClientId, EventId, new List<long> { 2, 3 }));
    }
}