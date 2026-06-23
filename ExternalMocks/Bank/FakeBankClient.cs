namespace ExternalMocks.Bank;

// Fake bank: lets tests drive the success / decline / timeout branches of payment.
public class FakeBankClient : IBankClient
{
    public bool TryCharge(long clientId, decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must not be negative", nameof(amount));

        // TODO (person 3): make the outcome configurable so corner-case tests can
        // force decline and timeout, not only success.
        return true;
    }
}
