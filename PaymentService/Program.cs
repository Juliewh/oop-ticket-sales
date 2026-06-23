using ExternalMocks.Bank;
using PaymentService.Services;
using Shared.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IBankClient, FakeBankClient>();
builder.Services.AddSingleton<IPaymentService, InMemoryPaymentService>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

app.Run();
