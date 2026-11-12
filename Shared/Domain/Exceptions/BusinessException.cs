namespace NovaScale.Platform.API.Shared.Domain.Exceptions;

public sealed class BusinessException(string code, int statusCode = 400) : Exception(code)
{
    public string Code { get; } = code;
    public int StatusCode { get; } = statusCode;
}
