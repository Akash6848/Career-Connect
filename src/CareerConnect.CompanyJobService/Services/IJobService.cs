using CareerConnect.CompanyJobService.Dto;
using CareerConnect.CompanyJobService.Enums;

namespace CareerConnect.CompanyJobService.Services;

public interface IJobService
{
    Task<JobDto> CreateJobAsync(CreateJobDto dto, long ownerId);
    Task<List<JobDto>> GetAllJobsAsync();
    Task<JobDto> GetJobByIdAsync(long id);
    Task<JobDetailedDto> GetJobDetailedAsync(long id);
    Task<List<JobDto>> GetJobsByCompanyAsync(long companyId);
    Task<List<JobDto>> GetJobsByCategoryAsync(long categoryId);
    Task<List<JobDto>> GetJobsSortedAsync(SortDirection direction);
    Task<JobDto> UpdateJobAsync(long id, UpdateJobDto dto, long ownerId, bool isAdmin);
    Task DeleteJobAsync(long id, long ownerId, bool isAdmin);

    Task<AppliedJobDto> ApplyToJobAsync(CreateAppliedJobDto dto, long userId);
    Task<List<AppliedJobDto>> GetApplicationsForJobAsync(long jobId);
}
