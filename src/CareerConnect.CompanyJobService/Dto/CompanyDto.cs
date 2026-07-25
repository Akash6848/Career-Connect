using System.ComponentModel.DataAnnotations;
using CareerConnect.CompanyJobService.Enums;
using CareerConnect.Shared.Validation;

namespace CareerConnect.CompanyJobService.Dto;

public class CreateCompanyDto
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required, MinLength(10)]
    public string Description { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 2)]
    public string Industry { get; set; } = string.Empty;

    [Required]
    public EmployeeRange EmployeeRange { get; set; }

    [Url]
    public string? WebsiteUrl { get; set; }
}

[AtLeastOneNotNull]
public class UpdateCompanyDto
{
    [StringLength(150, MinimumLength = 2)]
    public string? Name { get; set; }

    [MinLength(10)]
    public string? Description { get; set; }

    [StringLength(100, MinimumLength = 2)]
    public string? Industry { get; set; }

    public EmployeeRange? EmployeeRange { get; set; }

    [Url]
    public string? WebsiteUrl { get; set; }
}

public class CompanyDto
{
    public long Id { get; set; }
    public long OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public EmployeeRange EmployeeRange { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? Logo { get; set; }
    public string? Banner { get; set; }
}

public class CompanyDetailedDto : CompanyDto
{
    public List<CompanyLocationDto> Locations { get; set; } = [];
    public int NumJobs { get; set; }
}
