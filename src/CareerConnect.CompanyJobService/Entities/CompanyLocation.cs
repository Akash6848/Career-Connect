namespace CareerConnect.CompanyJobService.Entities;

public class CompanyLocation
{
    public long Id { get; set; }

    public long CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public bool IsRemote { get; set; }
}
