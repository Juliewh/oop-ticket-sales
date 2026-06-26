using BookingService.Services;
using ExternalMocks.Events;
using Shared.Web;
using PaymentService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IEventsClient, FakeEventsClient>();
builder.Services.AddSingleton<ExternalMocks.Bank.IBankClient, ExternalMocks.Bank.FakeBankClient>();
builder.Services.AddSingleton<IPaymentService, InMemoryPaymentService>();
builder.Services.AddSingleton<IPaymentGateway, PaymentServiceGateway>();
builder.Services.AddSingleton<IBookingService, InMemoryBookingService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

app.Run();