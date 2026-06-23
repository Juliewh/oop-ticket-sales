namespace Shared.Errors;

public class PaymentFailedException : DomainException
{
    public PaymentFailedException(string reason)
        : base($"Payment failed: {reason}")
    {
    }

    public override int StatusCode => 402;
}
