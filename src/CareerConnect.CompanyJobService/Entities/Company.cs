using CareerConnect.CompanyJobService.Enums;

namespace CareerConnect.CompanyJobService.Entities;

public class Company
{
    public long Id { get; set; }

    /// <summary>The UsersService user id that created/administers this company page.</summary>
    public long OwnerId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public EmployeeRange EmployeeRange { get; set; }
    public string? WebsiteUrl { get; set; }

    public List<CompanyLocation> Locations { get; set; } = [];
    public List<Job> Jobs { get; set; } = [];
    public List<CompanyFiles> Files { get; set; } = [];
}
