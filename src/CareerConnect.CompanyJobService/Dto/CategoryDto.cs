using System.ComponentModel.DataAnnotations;

namespace CareerConnect.CompanyJobService.Dto;

public class CreateJobCategoryDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
}

public class UpdateJobCategoryDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
}

public class JobCategoryDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
