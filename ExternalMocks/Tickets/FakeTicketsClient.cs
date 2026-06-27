namespace ExternalMocks.Tickets;

public class FakeTicketsClient : ITicketsClient
{
    private readonly List<long> _issuedTicketIds;
    private readonly bool _canIssueTickets;
    private long _lastTicketId;

    public FakeTicketsClient()
        : this(canIssueTickets: true)
    {
    }

    public FakeTicketsClient(bool canIssueTickets)
    {
        _issuedTicketIds = new List<long>();
        _canIssueTickets = canIssueTickets;
        _lastTicketId = 0;
    }

    public long IssueTicket(long clientId, long eventId, long seatId)
    {
        if (clientId <= 0)
            throw new ArgumentException("Client id must be positive", nameof(clientId));

        if (eventId <= 0)
            throw new ArgumentException("Event id must be positive", nameof(eventId));

        if (seatId <= 0)
            throw new ArgumentException("Seat id must be positive", nameof(seatId));

        if (!_canIssueTickets)
            throw new InvalidOperationException("Ticket service cannot issue tickets");

        var ticketId = ++_lastTicketId;
        _issuedTicketIds.Add(ticketId);

        return ticketId;
    }

    public IReadOnlyCollection<long> GetIssuedTicketIds()
        => _issuedTicketIds.ToList();
}
