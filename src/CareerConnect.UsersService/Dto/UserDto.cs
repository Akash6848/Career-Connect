namespace CareerConnect.UsersService.Dto;

public class UserDto
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class UserProfileDto : UserDto
{
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Website { get; set; }
    public string? ProfessionalSummary { get; set; }
    public string? HeadLine { get; set; }
    public DateOnly? Dob { get; set; }
    public string? Logo { get; set; }
    public string? Banner { get; set; }
}

public class UserFriendsDto
{
    public List<UserDto> UserFriends { get; set; } = [];
}
