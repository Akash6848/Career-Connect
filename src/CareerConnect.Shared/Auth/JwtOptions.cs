namespace CareerConnect.Shared.Auth;

/// <summary>
/// Bound from the "Jwt" config section. The same Secret/Issuer/Audience must be configured
/// identically across every service so a token issued by UsersService validates everywhere else -
/// there is no central auth server, each service independently validates the bearer token.
/// </summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60 * 24;
}
