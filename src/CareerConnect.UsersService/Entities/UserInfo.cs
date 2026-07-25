namespace CareerConnect.UsersService.Entities;

public class UserInfo
{
    public long Id { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Website { get; set; }
    public string? ProfessionalSummary { get; set; }
    public string? HeadLine { get; set; }
    public DateOnly Dob { get; set; }
}
