namespace ExternalMocks.Users;

public interface IUserClient
{
    bool IsRegistered(long clientId);
}
