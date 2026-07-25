namespace CareerConnect.CompanyJobService.Entities;

public class JobCategory
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<Job> Jobs { get; set; } = [];
}
