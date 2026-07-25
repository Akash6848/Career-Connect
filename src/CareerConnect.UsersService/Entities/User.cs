namespace CareerConnect.UsersService.Entities;

public class User
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public UserInfo? UserInfo { get; set; }

    public List<User> Friends { get; set; } = [];
    public List<Role> Roles { get; set; } = [];
    public List<UserFiles> Files { get; set; } = [];
    public List<Experience> Experiences { get; set; } = [];
}
