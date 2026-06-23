namespace Shared.Errors;

public class PaymentNotFoundException : DomainException
{
    public PaymentNotFoundException(long paymentId)
        : base($"Payment with id {paymentId} is not found")
    {
    }

    public override int StatusCode => 404;
}
