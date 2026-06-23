namespace ExternalMocks.Users;

// Fake User dependency: treats every positive client id as a registered user.
public class FakeUserClient : IUserClient
{
    public bool IsRegistered(long clientId)
        => clientId > 0;
}
