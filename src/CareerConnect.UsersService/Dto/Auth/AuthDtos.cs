using System.ComponentModel.DataAnnotations;

namespace CareerConnect.UsersService.Dto.Auth;

public class RegisterRequest
{
    [Required, StringLength(30, MinimumLength = 5)]
    public string Username { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [Required, StringLength(20, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(20, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public UserDto User { get; set; } = null!;
}
