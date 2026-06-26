namespace ExternalMocks.Bank;

public class FakeBankClient : IBankClient
{
    private readonly BankOutcome _outcome;

    public FakeBankClient()
        : this(BankOutcome.Success)
    {
    }

    public FakeBankClient(BankOutcome outcome)
    {
        if (outcome is BankOutcome.Undefined)
            throw new ArgumentException("Bank outcome must be defined", nameof(outcome));

        _outcome = outcome;
    }

    public bool TryCharge(long clientId, decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount must not be negative", nameof(amount));

        return _outcome switch
        {
            BankOutcome.Success => true,
            BankOutcome.Decline => false,
            BankOutcome.Timeout => throw new TimeoutException("Bank did not respond in time"),
            _ => throw new InvalidOperationException($"Unexpected bank outcome: {_outcome}"),
        };
    }
}