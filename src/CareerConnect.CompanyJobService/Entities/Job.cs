using CareerConnect.CompanyJobService.Enums;

namespace CareerConnect.CompanyJobService.Entities;

public class Job
{
    public long Id { get; set; }

    public long CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public long CategoryId { get; set; }
    public JobCategory Category { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EmploymentType EmploymentType { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public DateTime PostedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public List<AppliedJob> Applications { get; set; } = [];
}
