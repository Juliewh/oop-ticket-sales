using BookingService.Services;
using ExternalMocks.Events;
using Shared.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IEventsClient, FakeEventsClient>();
builder.Services.AddSingleton<IBookingService, InMemoryBookingService>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

app.Run();
