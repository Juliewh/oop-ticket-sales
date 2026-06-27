using BookingService.Services;
using ExternalMocks.Events;
using ExternalMocks.Users;
using Shared.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IEventsClient, FakeEventsClient>();
builder.Services.AddSingleton<IUserClient, FakeUserClient>();
builder.Services
    .AddHttpClient<IPaymentGateway, HttpPaymentGateway>(client =>
    {
        var baseUrl = builder.Configuration["PaymentService:BaseUrl"] ?? "http://localhost:5001/";
        client.BaseAddress = new Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(2);
    });
builder.Services.AddSingleton<IBookingService, InMemoryBookingService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

app.Run();
