namespace ExternalMocks.Tickets;

// Fake Tickets dependency: pretends to issue a ticket and hands back a fake id.
public class FakeTicketsClient : ITicketsClient
{
    private long _lastTicketId;

    public FakeTicketsClient()
    {
        _lastTicketId = 0;
    }

    public long IssueTicket(long clientId, long eventId, long seatId)
        => ++_lastTicketId;
}
