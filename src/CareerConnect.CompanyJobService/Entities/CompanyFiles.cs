using CareerConnect.CompanyJobService.Enums;

namespace CareerConnect.CompanyJobService.Entities;

public class CompanyFiles
{
    public long Id { get; set; }

    public long CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public CompanyFileType Type { get; set; }
    public string Link { get; set; } = string.Empty;
}
