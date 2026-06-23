using System.Linq.Expressions;
using PaymentService.Dto;
using PaymentService.Entities;

namespace PaymentService.Mapping;

public static class PaymentMapping
{
    public static Expression<Func<Payment, PaymentDto>> ToDto => payment => new PaymentDto
    {
        Id = payment.Id,
        Cost = payment.Cost,
        IsSucceed = payment.IsSucceed,
        TicketId = payment.BookingId,
        ClientId = payment.ClientId,
    };
}
