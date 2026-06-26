using PaymentService.Services;

namespace BookingService.Services;

public class PaymentServiceGateway : IPaymentGateway
{
    private readonly IPaymentService _paymentService;

    public PaymentServiceGateway(IPaymentService paymentService)
    {
        if (paymentService is null)
            throw new ArgumentNullException(nameof(paymentService));

        _paymentService = paymentService;
    }

    public bool TryPay(long clientId, long bookingId, decimal cost)
    {
        try
        {
            var payment = _paymentService.ProcessPayment(clientId, bookingId, cost);

            return payment.IsSucceed;
        }
        catch (Shared.Errors.PaymentFailedException)
        {
            return false;
        }
    }
}