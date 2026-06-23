namespace ExternalMocks.Tickets;

public interface ITicketsClient
{
    long IssueTicket(long clientId, long eventId, long seatId);
}
