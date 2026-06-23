using Microsoft.AspNetCore.Mvc;
using PaymentService.Dto;
using PaymentService.Mapping;
using PaymentService.Services;

namespace PaymentService.Controllers;

[ApiController]
[Route("payment")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        if (paymentService is null)
            throw new ArgumentNullException(nameof(paymentService));

        _paymentService = paymentService;
    }

    [HttpPost("add")]
    public PaymentDto Add(CreatePaymentRequest request)
    {
        var payment = _paymentService.ProcessPayment(request.ClientId, request.BookingId, request.Cost);

        return PaymentMapping.ToDto.Compile().Invoke(payment);
    }

    [HttpGet("get/{id}")]
    public PaymentDto Get(long id)
    {
        var payment = _paymentService.GetPayment(id);

        return PaymentMapping.ToDto.Compile().Invoke(payment);
    }

    [HttpGet("getAllByUserId/{userId}")]
    public IReadOnlyCollection<PaymentDto> GetAllByUserId(long userId)
        => _paymentService
            .GetPaymentsByClient(userId)
            .Select(PaymentMapping.ToDto.Compile())
            .ToList();

    [HttpDelete("{id}")]
    public void Delete(long id)
        => _paymentService.RemovePayment(id);
}
