using System.Net;
using CareerConnect.Shared.Dto;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CareerConnect.Shared.Exceptions;

/// <summary>
/// Single global exception handler used by every service (registered via
/// AddCareerConnectExceptionHandling): ApiException maps to its own status code and message,
/// anything else becomes a logged 500 with a consistent ErrorDetails body.
/// </summary>
public class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, message) = exception switch
        {
            ApiException apiException => (apiException.StatusCode, apiException.Message),
            _ => (HttpStatusCode.InternalServerError, exception.Message)
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception processing {Path}", httpContext.Request.Path);
        }

        var errorDetails = new ErrorDetails
        {
            Message = message,
            Details = httpContext.Request.Path,
            StatusCode = (int)statusCode
        };

        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(errorDetails, cancellationToken);

        return true;
    }
}
