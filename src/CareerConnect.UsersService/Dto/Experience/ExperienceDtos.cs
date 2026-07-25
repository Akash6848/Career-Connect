using System.ComponentModel.DataAnnotations;
using CareerConnect.Shared.Validation;

namespace CareerConnect.UsersService.Dto.Experience;

public class CreateExperienceDto
{
    [Required, MinLength(10)]
    public string Description { get; set; } = string.Empty;

    [Required, StringLength(255, MinimumLength = 3)]
    public string Company { get; set; } = string.Empty;

    [Required, StringLength(255, MinimumLength = 8)]
    public string Position { get; set; } = string.Empty;

    [Required, Range(0.01, float.MaxValue)]
    public float ExperienceInYears { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}

[AtLeastOneNotNull]
public class UpdateExperienceDto
{
    [MinLength(10)]
    public string? Description { get; set; }

    [StringLength(255, MinimumLength = 3)]
    public string? Company { get; set; }

    [StringLength(255, MinimumLength = 8)]
    public string? Position { get; set; }

    [Range(0.01, float.MaxValue)]
    public float? ExperienceInYears { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class ExperienceDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public float ExperienceInYears { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
