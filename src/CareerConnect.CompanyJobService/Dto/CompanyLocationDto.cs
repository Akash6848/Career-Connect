using System.ComponentModel.DataAnnotations;
using CareerConnect.Shared.Validation;

namespace CareerConnect.CompanyJobService.Dto;

public class CreateCompanyLocationDto
{
    [Required]
    public long CompanyId { get; set; }

    [Required, StringLength(255)]
    public string Address { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Country { get; set; } = string.Empty;

    public bool IsRemote { get; set; }
}

[AtLeastOneNotNull]
public class UpdateCompanyLocationDto
{
    [StringLength(255)]
    public string? Address { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    public bool? IsRemote { get; set; }
}

public class CompanyLocationDto
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsRemote { get; set; }
}
