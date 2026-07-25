using System.Net;
using System.Security.Claims;
using CareerConnect.Shared.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CareerConnect.Shared.Auth;

/// <summary>
/// Controllers' one-stop access to the caller's identity, read from the validated JWT claims on
/// HttpContext.User: user id, email, and role checks, with consistent 401/403 ApiExceptions when
/// a required claim is missing.
/// </summary>
public static class CurrentUserExtensions
{
    public static long GetUserId(this HttpContext httpContext)
    {
        var value = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (value is null || !long.TryParse(value, out var userId))
        {
            throw new ApiException(HttpStatusCode.Unauthorized, "Missing or invalid user id claim");
        }

        return userId;
    }

    public static string GetUserEmail(this HttpContext httpContext)
    {
        var email = httpContext.User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(email))
        {
            throw new ApiException(HttpStatusCode.Unauthorized, "Missing user email claim");
        }

        return email;
    }

    public static bool IsAdmin(this HttpContext httpContext) =>
        httpContext.User.IsInRole("ADMIN");

    public static void ShouldBeAdmin(this HttpContext httpContext)
    {
        if (!httpContext.IsAdmin())
        {
            throw new ApiException(HttpStatusCode.Forbidden, "This action requires admin privileges");
        }
    }
}
