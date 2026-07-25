using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CareerConnect.Shared.Auth;

public static class AuthServiceCollectionExtensions
{
    /// <summary>
    /// Wires up JWT bearer validation identically in every service. Any service configured with
    /// the same Jwt:Secret/Issuer/Audience values can validate tokens issued by
    /// CareerConnect.UsersService's /auth/login endpoint - no central auth server required.
    /// </summary>
    public static IServiceCollection AddCareerConnectJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
            ?? throw new InvalidOperationException("Missing required 'Jwt' configuration section");

        // Fail fast at startup with a clear message; HMAC-SHA256 needs a key of at least 256 bits,
        // and an empty secret would otherwise only surface as a cryptic crypto error on first request.
        if (string.IsNullOrWhiteSpace(jwtOptions.Secret) || jwtOptions.Secret.Length < 32)
        {
            throw new InvalidOperationException(
                "'Jwt:Secret' must be configured with at least 32 characters " +
                "(set it via user-secrets or environment variables; see README).");
        }

        services.AddSingleton(jwtOptions);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization();

        return services;
    }
}
