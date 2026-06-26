using BookingService.Entities;
using BookingService.Services;
using ExternalMocks.Bank;
using ExternalMocks.Events;
using ExternalMocks.Tickets;
using ExternalMocks.Users;
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
        var ticketsClient = new FakeTicketsClient();
        var userClient = new FakeUserClient(new[] { ClientId });
        var paymentService = new InMemoryPaymentService(bankClient, ticketsClient);
        var paymentGateway = new PaymentServiceGateway(paymentService);

        return new InMemoryBookingService(eventsClient, paymentGateway, userClient);
    }

    private static InMemoryBookingService CreateService(
        FakeEventsClient eventsClient,
        FakeUserClient userClient,
        BankOutcome bankOutcome = BankOutcome.Success)
    {
        var bankClient = new FakeBankClient(bankOutcome);
        var ticketsClient = new FakeTicketsClient();
        var paymentService = new InMemoryPaymentService(bankClient, ticketsClient);
        var paymentGateway = new PaymentServiceGateway(paymentService);

        return new InMemoryBookingService(eventsClient, paymentGateway, userClient);
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
    public void PayBooking_WhenBankTimesOut_CancelsBookingAndReleasesSeats()
    {
        var service = CreateService(BankOutcome.Timeout);
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

    [Fact]
    public void PayBooking_WhenBookingIsAlreadyPaid_Throws()
    {
        var service = CreateService(BankOutcome.Success);
        var booking = service.CreateBooking(ClientId, EventId, new List<long> { 1, 2 });

        service.PayBooking(booking.Id, Cost);

        Assert.Throws<InvalidOperationException>(() => service.PayBooking(booking.Id, Cost));
    }

    [Fact]
    public void CreateBooking_WhenUserIsNotRegistered_Throws()
    {
        var eventsClient = new FakeEventsClient();
        var userClient = new FakeUserClient(Array.Empty<long>());
        var service = CreateService(eventsClient, userClient);

        Assert.Throws<UserNotRegisteredException>(
            () => service.CreateBooking(ClientId, EventId, new List<long> { 1, 2 }));
    }

    [Fact]
    public void CreateBooking_WhenEventDoesNotExist_Throws()
    {
        var eventsClient = new FakeEventsClient();
        eventsClient.RemoveEvent(EventId);
        var userClient = new FakeUserClient(new[] { ClientId });
        var service = CreateService(eventsClient, userClient);

        Assert.Throws<EventNotFoundException>(
            () => service.CreateBooking(ClientId, EventId, new List<long> { 1, 2 }));
    }

    [Fact]
    public void CreateBooking_WhenEventIsInPast_Throws()
    {
        var eventsClient = new FakeEventsClient();
        eventsClient.AddEvent(EventId, DateTime.UtcNow.AddDays(-1));
        var userClient = new FakeUserClient(new[] { ClientId });
        var service = CreateService(eventsClient, userClient);

        Assert.Throws<EventUnavailableException>(
            () => service.CreateBooking(ClientId, EventId, new List<long> { 1, 2 }));
    }

    [Fact]
    public void CreateBooking_SameSeatOnDifferentEvents_IsAllowed()
    {
        var eventsClient = new FakeEventsClient();
        eventsClient.AddEvent(2, DateTime.UtcNow.AddDays(30));
        var userClient = new FakeUserClient(new[] { ClientId });
        var service = CreateService(eventsClient, userClient);

        var first = service.CreateBooking(ClientId, EventId, new List<long> { 1 });
        var second = service.CreateBooking(ClientId, 2, new List<long> { 1 });

        Assert.Equal(BookingStatus.Reserved, first.Status);
        Assert.Equal(BookingStatus.Reserved, second.Status);
    }
}
