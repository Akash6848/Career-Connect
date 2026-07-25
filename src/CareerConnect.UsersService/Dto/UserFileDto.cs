namespace CareerConnect.UsersService.Dto;

public class UserFileDto
{
    public string Type { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
}

public class UserResumeDto
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string Link { get; set; } = string.Empty;
}
