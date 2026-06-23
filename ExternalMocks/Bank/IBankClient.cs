namespace ExternalMocks.Bank;

public interface IBankClient
{
    bool TryCharge(long clientId, decimal amount);
}
