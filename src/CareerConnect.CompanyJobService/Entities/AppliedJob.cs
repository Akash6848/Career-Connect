namespace CareerConnect.CompanyJobService.Entities;

public class AppliedJob
{
    public long Id { get; set; }

    public long JobId { get; set; }
    public Job Job { get; set; } = null!;

    /// <summary>The UsersService user id who applied.</summary>
    public long UserId { get; set; }

    public string ResumeLink { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
}
