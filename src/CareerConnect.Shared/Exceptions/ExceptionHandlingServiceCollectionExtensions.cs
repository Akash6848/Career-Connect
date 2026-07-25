using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace CareerConnect.Shared.Exceptions;

public static class ExceptionHandlingServiceCollectionExtensions
{
    /// <summary>
    /// Registers the shared ApiExceptionHandler and normalizes [ApiController] model-validation
    /// failures into a flat field-name -> message dictionary so every service returns validation
    /// errors in the same shape.
    /// </summary>
    public static IServiceCollection AddCareerConnectExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<ApiExceptionHandler>();
        services.AddProblemDetails();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors.First().ErrorMessage);

                return new BadRequestObjectResult(errors);
            };
        });

        return services;
    }
}
