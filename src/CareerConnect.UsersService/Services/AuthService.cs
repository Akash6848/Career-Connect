using System.Net;
using CareerConnect.Shared.Exceptions;
using CareerConnect.UsersService.Data;
using CareerConnect.UsersService.Dto;
using CareerConnect.UsersService.Dto.Auth;
using CareerConnect.UsersService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.UsersService.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}

/// <summary>
/// Sole token issuer for the platform: registration assigns the default "USER" role, passwords are
/// BCrypt-hashed, and both register and login return a JWT that every other service validates
/// against the same shared Jwt configuration.
/// </summary>
public class AuthService(UsersDbContext db, JwtTokenGenerator tokenGenerator) : IAuthService
{
    private const string DefaultRoleName = "USER";

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await db.Users.AnyAsync(u => u.Username == request.Username))
        {
            throw new ApiException(HttpStatusCode.BadRequest, "username already exists");
        }

        if (await db.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new ApiException(HttpStatusCode.BadRequest, "email already registered");
        }

        var defaultRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == DefaultRoleName);
        if (defaultRole is null)
        {
            defaultRole = new Role { Name = DefaultRoleName };
            db.Roles.Add(defaultRole);
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Roles = [defaultRole]
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await db.Users
            .Include(u => u.Roles)
            .FirstOrDefaultAsync(u => u.Email == request.Email)
            ?? throw new ApiException(HttpStatusCode.Unauthorized, "invalid email or password");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            throw new ApiException(HttpStatusCode.Unauthorized, "invalid email or password");
        }

        return BuildAuthResponse(user);
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var (token, expiresAt) = tokenGenerator.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            ExpiresAtUtc = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            }
        };
    }
}
