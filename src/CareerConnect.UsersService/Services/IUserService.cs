using CareerConnect.UsersService.Dto;
using Microsoft.AspNetCore.Http;

namespace CareerConnect.UsersService.Services;

public interface IUserService
{
    Task<UserDto> GetUserByIdAsync(long userId);
    Task<List<UserDto>> GetUsersAsync();
    Task<string> DeleteUserAsync(long id);
    Task<UserDto> UpdateUserAsync(UpdateUserDto dto, string email);
    Task<UserProfileDto> GetProfileAsync(string email);
    Task<UserProfileDto> GetProfileByIdAsync(long userId);
    Task<List<UserDto>> GetUserFriendsAsync(string email);
    Task<string> UploadUserFileAsync(IFormFile file, string fileType, long userId);
    Task<List<UserFileDto>> GetAllFilesAsync(long userId);
    Task<UserFileDto> GetFileByTypeAsync(string fileType, long userId);
    Task<List<UserResumeDto>> GetUserResumesAsync(long userId);
}
