namespace ExternalMocks.Users;

public class FakeUserClient : IUserClient
{
    private readonly HashSet<long> _registeredClientIds;

    public FakeUserClient()
        : this(new[] { 1L })
    {
    }

    public FakeUserClient(IReadOnlyCollection<long> registeredClientIds)
    {
        if (registeredClientIds is null)
            throw new ArgumentNullException(nameof(registeredClientIds));

        _registeredClientIds = registeredClientIds.ToHashSet();
    }

    public void Register(long clientId)
    {
        if (clientId <= 0)
            throw new ArgumentException("Client id must be positive", nameof(clientId));

        _registeredClientIds.Add(clientId);
    }

    public void Unregister(long clientId)
    {
        if (clientId <= 0)
            throw new ArgumentException("Client id must be positive", nameof(clientId));

        _registeredClientIds.Remove(clientId);
    }

    public bool IsRegistered(long clientId)
        => _registeredClientIds.Contains(clientId);
}
