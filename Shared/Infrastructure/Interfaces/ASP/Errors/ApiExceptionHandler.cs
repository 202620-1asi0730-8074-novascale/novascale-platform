using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using NovaScale.Platform.API.Resources;
using NovaScale.Platform.API.Shared.Domain.Exceptions;

namespace NovaScale.Platform.API.Shared.Infrastructure.Interfaces.ASP.Errors;

public sealed class ApiExceptionHandler(IProblemDetailsService problems, IStringLocalizer<SharedResource> localizer,
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested) return false;
        var code = "UnexpectedError";
        var status = StatusCodes.Status500InternalServerError;
        if (exception is BusinessException business) { code = business.Code; status = business.StatusCode; }
        else if (exception is DbUpdateException)
        {
            status = StatusCodes.Status500InternalServerError;
            code = "UnexpectedError";
            for (Exception? inner = exception; inner is not null; inner = inner.InnerException)
                if (inner.GetType().GetProperty("Number")?.GetValue(inner) is int number)
                    (code, status) = number switch { 1062 => ("Duplicate", 409), 1452 => ("InvalidReference", 400), 1451 => ("InUse", 409), _ => ("UnexpectedError", 500) };
        }
        if (status >= 500) logger.LogError(exception, "Request failed: {TraceId}", httpContext.TraceIdentifier);
        httpContext.Response.StatusCode = status;
        return await problems.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = status, Title = localizer[code], Instance = httpContext.Request.Path,
                Extensions = { ["code"] = code, ["traceId"] = httpContext.TraceIdentifier }
            }
        });
    }
}
