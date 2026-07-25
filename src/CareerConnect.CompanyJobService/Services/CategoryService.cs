using System.Net;
using CareerConnect.CompanyJobService.Data;
using CareerConnect.CompanyJobService.Dto;
using CareerConnect.CompanyJobService.Entities;
using CareerConnect.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.CompanyJobService.Services;

public class CategoryService(CompanyJobDbContext db) : ICategoryService
{
    public async Task<JobCategoryDto> CreateCategoryAsync(CreateJobCategoryDto dto)
    {
        if (await db.JobCategories.AnyAsync(c => c.Name == dto.Name))
        {
            throw new ApiException(HttpStatusCode.BadRequest, "a category with this name already exists");
        }

        var category = new JobCategory { Name = dto.Name };
        db.JobCategories.Add(category);
        await db.SaveChangesAsync();

        return ToDto(category);
    }

    public async Task<List<JobCategoryDto>> GetAllCategoriesAsync() =>
        await db.JobCategories.Select(c => ToDto(c)).ToListAsync();

    public async Task<JobCategoryDto> GetCategoryByIdAsync(long id) => ToDto(await GetOrThrowAsync(id));

    public async Task<JobCategoryDto> UpdateCategoryAsync(long id, UpdateJobCategoryDto dto)
    {
        var category = await GetOrThrowAsync(id);
        category.Name = dto.Name;
        await db.SaveChangesAsync();

        return ToDto(category);
    }

    public async Task DeleteCategoryAsync(long id)
    {
        var category = await GetOrThrowAsync(id);

        // Jobs reference categories with a Restrict FK; surface a clear 400 rather than
        // letting the database constraint violation bubble up as a 500.
        if (await db.Jobs.AnyAsync(j => j.CategoryId == id))
        {
            throw new ApiException(HttpStatusCode.BadRequest,
                "cannot delete a category that still has jobs assigned to it");
        }

        db.JobCategories.Remove(category);
        await db.SaveChangesAsync();
    }

    private async Task<JobCategory> GetOrThrowAsync(long id) =>
        await db.JobCategories.FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"category with id {id} was not found");

    private static JobCategoryDto ToDto(JobCategory category) => new() { Id = category.Id, Name = category.Name };
}
