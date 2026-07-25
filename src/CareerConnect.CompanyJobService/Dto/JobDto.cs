using System.ComponentModel.DataAnnotations;
using CareerConnect.CompanyJobService.Enums;
using CareerConnect.Shared.Validation;

namespace CareerConnect.CompanyJobService.Dto;

public class CreateJobDto
{
    [Required]
    public long CompanyId { get; set; }

    [Required]
    public long CategoryId { get; set; }

    [Required, StringLength(150, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(20)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public EmploymentType EmploymentType { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MinSalary { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MaxSalary { get; set; }
}

[AtLeastOneNotNull]
public class UpdateJobDto
{
    public long? CategoryId { get; set; }

    [StringLength(150, MinimumLength = 5)]
    public string? Title { get; set; }

    [MinLength(20)]
    public string? Description { get; set; }

    public EmploymentType? EmploymentType { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MinSalary { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MaxSalary { get; set; }

    public bool? IsActive { get; set; }
}

public class JobDto
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EmploymentType EmploymentType { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public DateTime PostedAt { get; set; }
    public bool IsActive { get; set; }
}

public class JobDetailedDto : JobDto
{
    public string CompanyName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int NumApplicants { get; set; }
}
