using CareerConnect.CompanyJobService.Dto;

namespace CareerConnect.CompanyJobService.Services;

public interface ICategoryService
{
    Task<JobCategoryDto> CreateCategoryAsync(CreateJobCategoryDto dto);
    Task<List<JobCategoryDto>> GetAllCategoriesAsync();
    Task<JobCategoryDto> GetCategoryByIdAsync(long id);
    Task<JobCategoryDto> UpdateCategoryAsync(long id, UpdateJobCategoryDto dto);
    Task DeleteCategoryAsync(long id);
}
