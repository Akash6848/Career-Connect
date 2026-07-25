using CareerConnect.UsersService.Enums;

namespace CareerConnect.UsersService.Entities;

public class UserFiles
{
    public long Id { get; set; }
    public string? Name { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public UserFileType Type { get; set; }
    public string Link { get; set; } = string.Empty;
}
