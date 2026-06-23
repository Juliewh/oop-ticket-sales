namespace Shared.Errors;

public record ErrorResponse
{
    public string Message { get; init; } = string.Empty;

    public string? Field { get; init; }
}
