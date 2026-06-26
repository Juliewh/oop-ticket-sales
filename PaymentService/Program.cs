using ExternalMocks.Bank;
using ExternalMocks.Tickets;
using PaymentService.Services;
using Shared.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IBankClient>(_ =>
{
    var configuredOutcome = builder.Configuration["Bank:Outcome"];

    if (Enum.TryParse<BankOutcome>(configuredOutcome, ignoreCase: true, out var outcome))
        return new FakeBankClient(outcome);

    return new FakeBankClient();
});
builder.Services.AddSingleton<ITicketsClient, FakeTicketsClient>();
builder.Services.AddSingleton<IPaymentService, InMemoryPaymentService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

app.Run();
