using System.Net;
using CareerConnect.CompanyJobService.Data;
using CareerConnect.CompanyJobService.Dto;
using CareerConnect.CompanyJobService.Entities;
using CareerConnect.CompanyJobService.Enums;
using CareerConnect.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.CompanyJobService.Services;

public class JobService(CompanyJobDbContext db) : IJobService
{
    public async Task<JobDto> CreateJobAsync(CreateJobDto dto, long ownerId)
    {
        var company = await db.Companies.FirstOrDefaultAsync(c => c.Id == dto.CompanyId)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"company with id {dto.CompanyId} was not found");

        if (company.OwnerId != ownerId)
        {
            throw new ApiException(HttpStatusCode.Forbidden, "You do not manage this company");
        }

        var categoryExists = await db.JobCategories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists) throw new ApiException(HttpStatusCode.NotFound, $"category with id {dto.CategoryId} was not found");

        var job = new Job
        {
            CompanyId = dto.CompanyId,
            CategoryId = dto.CategoryId,
            Title = dto.Title,
            Description = dto.Description,
            EmploymentType = dto.EmploymentType,
            MinSalary = dto.MinSalary,
            MaxSalary = dto.MaxSalary,
            PostedAt = DateTime.UtcNow,
            IsActive = true
        };

        db.Jobs.Add(job);
        await db.SaveChangesAsync();

        return ToDto(job);
    }

    public async Task<List<JobDto>> GetAllJobsAsync() =>
        await db.Jobs.Select(j => ToDto(j)).ToListAsync();

    public async Task<JobDto> GetJobByIdAsync(long id) => ToDto(await GetOrThrowAsync(id));

    public async Task<JobDetailedDto> GetJobDetailedAsync(long id)
    {
        var job = await db.Jobs
            .Include(j => j.Company)
            .Include(j => j.Category)
            .FirstOrDefaultAsync(j => j.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"job with id {id} was not found");

        var numApplicants = await db.AppliedJobs.CountAsync(a => a.JobId == id);

        var dto = ToDto(job);
        return new JobDetailedDto
        {
            Id = dto.Id,
            CompanyId = dto.CompanyId,
            CategoryId = dto.CategoryId,
            Title = dto.Title,
            Description = dto.Description,
            EmploymentType = dto.EmploymentType,
            MinSalary = dto.MinSalary,
            MaxSalary = dto.MaxSalary,
            PostedAt = dto.PostedAt,
            IsActive = dto.IsActive,
            CompanyName = job.Company.Name,
            CategoryName = job.Category.Name,
            NumApplicants = numApplicants
        };
    }

    public async Task<List<JobDto>> GetJobsByCompanyAsync(long companyId) =>
        await db.Jobs.Where(j => j.CompanyId == companyId).Select(j => ToDto(j)).ToListAsync();

    public async Task<List<JobDto>> GetJobsByCategoryAsync(long categoryId) =>
        await db.Jobs.Where(j => j.CategoryId == categoryId).Select(j => ToDto(j)).ToListAsync();

    public async Task<List<JobDto>> GetJobsSortedAsync(SortDirection direction)
    {
        var query = direction == SortDirection.Ascending
            ? db.Jobs.OrderBy(j => j.PostedAt)
            : db.Jobs.OrderByDescending(j => j.PostedAt);

        return await query.Select(j => ToDto(j)).ToListAsync();
    }

    public async Task<JobDto> UpdateJobAsync(long id, UpdateJobDto dto, long ownerId, bool isAdmin)
    {
        var job = await db.Jobs.Include(j => j.Company).FirstOrDefaultAsync(j => j.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"job with id {id} was not found");

        if (job.Company.OwnerId != ownerId && !isAdmin)
        {
            throw new ApiException(HttpStatusCode.Forbidden, "You do not manage this job's company");
        }

        if (dto.CategoryId is not null)
        {
            var categoryExists = await db.JobCategories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists) throw new ApiException(HttpStatusCode.NotFound, $"category with id {dto.CategoryId} was not found");
            job.CategoryId = dto.CategoryId.Value;
        }

        if (dto.Title is not null) job.Title = dto.Title;
        if (dto.Description is not null) job.Description = dto.Description;
        if (dto.EmploymentType is not null) job.EmploymentType = dto.EmploymentType.Value;
        if (dto.MinSalary is not null) job.MinSalary = dto.MinSalary;
        if (dto.MaxSalary is not null) job.MaxSalary = dto.MaxSalary;
        if (dto.IsActive is not null) job.IsActive = dto.IsActive.Value;

        await db.SaveChangesAsync();

        return ToDto(job);
    }

    public async Task DeleteJobAsync(long id, long ownerId, bool isAdmin)
    {
        var job = await db.Jobs.Include(j => j.Company).FirstOrDefaultAsync(j => j.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"job with id {id} was not found");

        if (job.Company.OwnerId != ownerId && !isAdmin)
        {
            throw new ApiException(HttpStatusCode.Forbidden, "You do not manage this job's company");
        }

        db.Jobs.Remove(job);
        await db.SaveChangesAsync();
    }

    public async Task<AppliedJobDto> ApplyToJobAsync(CreateAppliedJobDto dto, long userId)
    {
        var jobExists = await db.Jobs.AnyAsync(j => j.Id == dto.JobId);
        if (!jobExists) throw new ApiException(HttpStatusCode.NotFound, $"job with id {dto.JobId} was not found");

        if (await db.AppliedJobs.AnyAsync(a => a.JobId == dto.JobId && a.UserId == userId))
        {
            throw new ApiException(HttpStatusCode.BadRequest, "You have already applied to this job");
        }

        var application = new AppliedJob
        {
            JobId = dto.JobId,
            UserId = userId,
            ResumeLink = dto.ResumeLink,
            AppliedAt = DateTime.UtcNow
        };

        db.AppliedJobs.Add(application);
        await db.SaveChangesAsync();

        return ToAppliedDto(application);
    }

    public async Task<List<AppliedJobDto>> GetApplicationsForJobAsync(long jobId) =>
        await db.AppliedJobs.Where(a => a.JobId == jobId).Select(a => ToAppliedDto(a)).ToListAsync();

    private async Task<Job> GetOrThrowAsync(long id) =>
        await db.Jobs.FirstOrDefaultAsync(j => j.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"job with id {id} was not found");

    private static JobDto ToDto(Job job) => new()
    {
        Id = job.Id,
        CompanyId = job.CompanyId,
        CategoryId = job.CategoryId,
        Title = job.Title,
        Description = job.Description,
        EmploymentType = job.EmploymentType,
        MinSalary = job.MinSalary,
        MaxSalary = job.MaxSalary,
        PostedAt = job.PostedAt,
        IsActive = job.IsActive
    };

    private static AppliedJobDto ToAppliedDto(AppliedJob applied) => new()
    {
        Id = applied.Id,
        JobId = applied.JobId,
        UserId = applied.UserId,
        ResumeLink = applied.ResumeLink,
        AppliedAt = applied.AppliedAt
    };
}
