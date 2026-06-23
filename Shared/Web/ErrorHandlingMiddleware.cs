using Microsoft.AspNetCore.Http;
using Shared.Errors;

namespace Shared.Web;

public class ErrorHandlingMiddleware
{
    private const int BadRequestStatusCode = 400;
    private const int ServerErrorStatusCode = 500;

    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        if (next is null)
            throw new ArgumentNullException(nameof(next));

        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException exception)
        {
            await WriteResponse(context, exception.StatusCode, exception.Message, field: null);
        }
        catch (ArgumentException exception)
        {
            await WriteResponse(context, BadRequestStatusCode, exception.Message, exception.ParamName);
        }
        catch (Exception exception)
        {
            await WriteResponse(context, ServerErrorStatusCode, exception.Message, field: null);
        }
    }

    private static async Task WriteResponse(HttpContext context, int statusCode, string message, string? field)
    {
        var response = new ErrorResponse
        {
            Message = message,
            Field = field,
        };

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(response);
    }
}
