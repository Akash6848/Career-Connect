using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CareerConnect.Shared.Auth;
using CareerConnect.UsersService.Entities;
using Microsoft.IdentityModel.Tokens;

namespace CareerConnect.UsersService.Services;

public class JwtTokenGenerator(JwtOptions jwtOptions)
{
    public (string Token, DateTime ExpiresAtUtc) GenerateToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(jwtOptions.ExpiryMinutes);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Username)
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
