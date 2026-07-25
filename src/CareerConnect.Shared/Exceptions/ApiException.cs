using System.Net;

namespace CareerConnect.Shared.Exceptions;

public class ApiException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
