using System.Net;
using CareerConnect.Shared.Clients;
using CareerConnect.Shared.Exceptions;
using CareerConnect.UsersService.Data;
using CareerConnect.UsersService.Dto;
using CareerConnect.UsersService.Entities;
using CareerConnect.UsersService.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.UsersService.Services;

public class UserService(UsersDbContext db, IFileServiceClient fileServiceClient) : IUserService
{
    private const string UsernameAlreadyExists = "username already exists";
    private const string UserDoesNotExist = "user does not exist";

    public async Task<UserDto> GetUserByIdAsync(long userId)
    {
        var user = await db.Users.FindAsync(userId)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"user with id {userId} was not found");

        return ToUserDto(user);
    }

    public async Task<List<UserDto>> GetUsersAsync() =>
        await db.Users.Select(u => ToUserDto(u)).ToListAsync();

    public async Task<string> DeleteUserAsync(long id)
    {
        var user = await db.Users.FindAsync(id)
            ?? throw new ApiException(HttpStatusCode.NotFound, UserDoesNotExist);

        db.Users.Remove(user);
        await db.SaveChangesAsync();

        return "User deleted successfully";
    }

    public async Task<UserDto> UpdateUserAsync(UpdateUserDto dto, string email)
    {
        var user = await GetUserByEmailOrThrowAsync(db.Users, email);

        if (dto.Username is not null && dto.Username != user.Username)
        {
            var usernameTaken = await db.Users.AnyAsync(u => u.Username == dto.Username);
            if (usernameTaken) throw new ApiException(HttpStatusCode.BadRequest, UsernameAlreadyExists);

            user.Username = dto.Username;
        }

        if (dto.Password is not null) user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        if (dto.FirstName is not null) user.FirstName = dto.FirstName;
        if (dto.LastName is not null) user.LastName = dto.LastName;

        await db.SaveChangesAsync();

        return ToUserDto(user);
    }

    public async Task<UserProfileDto> GetProfileAsync(string email)
    {
        var user = await GetUserByEmailOrThrowAsync(db.Users.Include(u => u.UserInfo), email);

        return await MapUserToProfileDtoAsync(user);
    }

    public async Task<UserProfileDto> GetProfileByIdAsync(long userId)
    {
        var user = await db.Users
            .Include(u => u.UserInfo)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"user with id {userId} was not found");

        return await MapUserToProfileDtoAsync(user);
    }

    public async Task<List<UserDto>> GetUserFriendsAsync(string email)
    {
        var user = await GetUserByEmailOrThrowAsync(db.Users.Include(u => u.Friends), email);

        return user.Friends.Select(ToUserDto).ToList();
    }

    public async Task<string> UploadUserFileAsync(IFormFile file, string fileType, long userId)
    {
        if (!Enum.TryParse<UserFileType>(fileType, ignoreCase: true, out var userFileType))
        {
            throw new ApiException(HttpStatusCode.BadRequest,
                $"Invalid File Type. Only {UserFileType.Logo}, {UserFileType.Banner}, {UserFileType.Resume} are supported");
        }

        string url;
        try
        {
            await using var stream = file.OpenReadStream();
            url = await fileServiceClient.UploadFileAsync(new Refit.StreamPart(stream, file.FileName, file.ContentType));
        }
        catch (Exception)
        {
            throw new ApiException(HttpStatusCode.InternalServerError, "Error while uploading file");
        }

        if (userFileType != UserFileType.Resume)
        {
            var existing = await db.UserFiles
                .FirstOrDefaultAsync(f => f.UserId == userId && f.Type == userFileType);

            if (existing is not null)
            {
                existing.Link = url;
                await db.SaveChangesAsync();
                return url;
            }
        }

        db.UserFiles.Add(new UserFiles
        {
            UserId = userId,
            Type = userFileType,
            Link = url,
            Name = file.FileName
        });
        await db.SaveChangesAsync();

        return url;
    }

    public async Task<List<UserFileDto>> GetAllFilesAsync(long userId) =>
        await db.UserFiles
            .Where(f => f.UserId == userId)
            .Select(f => new UserFileDto { Type = f.Type.ToString(), Link = f.Link })
            .ToListAsync();

    public async Task<UserFileDto> GetFileByTypeAsync(string fileType, long userId)
    {
        if (!Enum.TryParse<UserFileType>(fileType, ignoreCase: true, out var userFileType))
        {
            throw new ApiException(HttpStatusCode.BadRequest,
                $"Invalid File Type. Only {UserFileType.Logo}, {UserFileType.Banner}, {UserFileType.Resume} are supported");
        }

        var userFile = await db.UserFiles.FirstOrDefaultAsync(f => f.UserId == userId && f.Type == userFileType)
            ?? throw new ApiException(HttpStatusCode.NotFound, "user file was not found");

        return new UserFileDto { Type = userFile.Type.ToString(), Link = userFile.Link };
    }

    public async Task<List<UserResumeDto>> GetUserResumesAsync(long userId) =>
        await db.UserFiles
            .Where(f => f.UserId == userId && f.Type == UserFileType.Resume)
            .Select(f => new UserResumeDto { Id = f.Id, Name = f.Name, Link = f.Link })
            .ToListAsync();

    private async Task<UserProfileDto> MapUserToProfileDtoAsync(User user)
    {
        var dto = ToUserProfileDto(user);

        if (user.UserInfo is not null)
        {
            dto.Address = user.UserInfo.Address;
            dto.ZipCode = user.UserInfo.ZipCode;
            dto.City = user.UserInfo.City;
            dto.Country = user.UserInfo.Country;
            dto.Website = user.UserInfo.Website;
            dto.ProfessionalSummary = user.UserInfo.ProfessionalSummary;
            dto.HeadLine = user.UserInfo.HeadLine;
            dto.Dob = user.UserInfo.Dob;
        }

        var logo = await db.UserFiles.FirstOrDefaultAsync(f => f.UserId == user.Id && f.Type == UserFileType.Logo);
        var banner = await db.UserFiles.FirstOrDefaultAsync(f => f.UserId == user.Id && f.Type == UserFileType.Banner);

        if (logo is not null) dto.Logo = logo.Link;
        if (banner is not null) dto.Banner = banner.Link;

        return dto;
    }

    /// <summary>
    /// The token being valid doesn't guarantee the user still exists (e.g. deleted by an admin
    /// after the JWT was issued) - resolve that to a clean 401 instead of an unhandled 500.
    /// </summary>
    private static async Task<User> GetUserByEmailOrThrowAsync(IQueryable<User> users, string email) =>
        await users.FirstOrDefaultAsync(u => u.Email == email)
            ?? throw new ApiException(HttpStatusCode.Unauthorized, "user account no longer exists");

    private static UserDto ToUserDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName
    };

    private static UserProfileDto ToUserProfileDto(User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName
    };
}
