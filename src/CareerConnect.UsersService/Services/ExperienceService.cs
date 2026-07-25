using System.Net;
using CareerConnect.Shared.Exceptions;
using CareerConnect.UsersService.Data;
using CareerConnect.UsersService.Dto.Experience;
using CareerConnect.UsersService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.UsersService.Services;

public class ExperienceService(UsersDbContext db) : IExperienceService
{
    public async Task CreateExperienceAsync(CreateExperienceDto dto, long userId)
    {
        db.Experiences.Add(new Experience
        {
            UserId = userId,
            Description = dto.Description,
            ExperienceInYears = dto.ExperienceInYears,
            Company = dto.Company,
            Position = dto.Position,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        });

        await db.SaveChangesAsync();
    }

    public async Task UpdateExperienceAsync(UpdateExperienceDto dto, long experienceId, long userId)
    {
        var experience = await GetExperienceOrThrowAsync(experienceId);

        if (experience.UserId != userId)
        {
            throw new ApiException(HttpStatusCode.BadRequest, "You cannot update someone else's experience");
        }

        if (dto.ExperienceInYears is not null) experience.ExperienceInYears = dto.ExperienceInYears.Value;
        if (dto.Company is not null) experience.Company = dto.Company;
        if (dto.Description is not null) experience.Description = dto.Description;
        if (dto.Position is not null) experience.Position = dto.Position;
        if (dto.StartDate is not null) experience.StartDate = dto.StartDate.Value;
        if (dto.EndDate is not null) experience.EndDate = dto.EndDate;

        await db.SaveChangesAsync();
    }

    public async Task<List<ExperienceDto>> GetAllExperiencesByUserIdAsync(long userId) =>
        await db.Experiences
            .Where(e => e.UserId == userId)
            .Select(e => ToDto(e))
            .ToListAsync();

    public async Task<ExperienceDto> GetExperienceByIdAsync(long experienceId) =>
        ToDto(await GetExperienceOrThrowAsync(experienceId));

    public async Task DeleteExperienceByIdAsync(long experienceId, long userId, bool isAdmin)
    {
        var experience = await GetExperienceOrThrowAsync(experienceId);

        if (experience.UserId != userId && !isAdmin)
        {
            throw new ApiException(HttpStatusCode.BadRequest, "You cannot delete someone else's experience");
        }

        db.Experiences.Remove(experience);
        await db.SaveChangesAsync();
    }

    private async Task<Experience> GetExperienceOrThrowAsync(long experienceId) =>
        await db.Experiences.FirstOrDefaultAsync(e => e.Id == experienceId)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"user experience with id {experienceId} was not found");

    private static ExperienceDto ToDto(Experience experience) => new()
    {
        Id = experience.Id,
        UserId = experience.UserId,
        Company = experience.Company,
        Position = experience.Position,
        Description = experience.Description,
        ExperienceInYears = experience.ExperienceInYears,
        StartDate = experience.StartDate,
        EndDate = experience.EndDate
    };
}
