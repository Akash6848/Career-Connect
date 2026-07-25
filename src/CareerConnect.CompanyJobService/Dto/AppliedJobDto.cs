using System.ComponentModel.DataAnnotations;

namespace CareerConnect.CompanyJobService.Dto;

public class CreateAppliedJobDto
{
    [Required]
    public long JobId { get; set; }

    [Required]
    public string ResumeLink { get; set; } = string.Empty;
}

public class AppliedJobDto
{
    public long Id { get; set; }
    public long JobId { get; set; }
    public long UserId { get; set; }
    public string ResumeLink { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
}
