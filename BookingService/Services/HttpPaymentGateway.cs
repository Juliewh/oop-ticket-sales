using System.Net.Http.Json;
using PaymentService.Dto;

namespace BookingService.Services;

public class HttpPaymentGateway : IPaymentGateway
{
    private const int MaxAttempts = 2;

    private readonly HttpClient _httpClient;

    public HttpPaymentGateway(HttpClient httpClient)
    {
        if (httpClient is null)
            throw new ArgumentNullException(nameof(httpClient));

        _httpClient = httpClient;
    }

    public bool TryPay(long clientId, long bookingId, long eventId, IReadOnlyCollection<long> seatIds, decimal cost)
    {
        var request = new CreatePaymentRequest
        {
            ClientId = clientId,
            BookingId = bookingId,
            EventId = eventId,
            SeatIds = seatIds,
            Cost = cost,
        };

        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                using var response = _httpClient
                    .PostAsJsonAsync("payment/add", request)
                    .GetAwaiter()
                    .GetResult();

                if (response.IsSuccessStatusCode)
                    return true;

                if ((int)response.StatusCode < 500 || attempt == MaxAttempts)
                    return false;
            }
            catch (TaskCanceledException) when (attempt < MaxAttempts)
            {
            }
            catch (HttpRequestException) when (attempt < MaxAttempts)
            {
            }
            catch (TaskCanceledException)
            {
                return false;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        return false;
    }
}
