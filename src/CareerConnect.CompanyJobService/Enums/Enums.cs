namespace CareerConnect.CompanyJobService.Enums;

/// <summary>
/// Company size bands shown on a company page: "1-10", "11-50", "50-500", "500+".
/// </summary>
public enum EmployeeRange
{
    OneToTen,
    ElevenToFifty,
    FiftyToFiveHundred,
    FiveHundredPlus
}

public enum EmploymentType
{
    FullTime,
    PartTime,
    Contract,
    Internship
}

public enum CompanyFileType
{
    Logo,
    Banner
}

public enum SortDirection
{
    Ascending,
    Descending
}
