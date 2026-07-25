using CareerConnect.UsersService.Dto.Experience;

namespace CareerConnect.UsersService.Services;

public interface IExperienceService
{
    Task CreateExperienceAsync(CreateExperienceDto dto, long userId);
    Task UpdateExperienceAsync(UpdateExperienceDto dto, long experienceId, long userId);
    Task<List<ExperienceDto>> GetAllExperiencesByUserIdAsync(long userId);
    Task<ExperienceDto> GetExperienceByIdAsync(long experienceId);
    Task DeleteExperienceByIdAsync(long experienceId, long userId, bool isAdmin);
}
