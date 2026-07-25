using CareerConnect.Shared.Auth;
using CareerConnect.UsersService.Dto;
using CareerConnect.UsersService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.UsersService.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<ActionResult<string>> UploadUserFile(IFormFile file, [FromForm] string fileType)
    {
        var userId = HttpContext.GetUserId();
        var url = await userService.UploadUserFileAsync(file, fileType, userId);
        return Ok(url);
    }

    [HttpGet("files")]
    public async Task<ActionResult<List<UserFileDto>>> GetAllUserFiles()
    {
        var userId = HttpContext.GetUserId();
        return Ok(await userService.GetAllFilesAsync(userId));
    }

    [HttpGet("files/{type}")]
    public async Task<ActionResult<UserFileDto>> GetFileByType(string type)
    {
        var userId = HttpContext.GetUserId();
        return Ok(await userService.GetFileByTypeAsync(type, userId));
    }

    [HttpGet("resumes")]
    public async Task<ActionResult<List<UserResumeDto>>> GetUserResumes()
    {
        var userId = HttpContext.GetUserId();
        return Ok(await userService.GetUserResumesAsync(userId));
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        HttpContext.ShouldBeAdmin();
        return Ok(await userService.GetUsersAsync());
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UserDto>> GetUserById(long id) =>
        Ok(await userService.GetUserByIdAsync(id));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<string>> DeleteUser(long id)
    {
        HttpContext.ShouldBeAdmin();
        return Ok(await userService.DeleteUserAsync(id));
    }

    [HttpPut]
    public async Task<ActionResult<UserDto>> UpdateUser([FromBody] UpdateUserDto dto)
    {
        var email = HttpContext.GetUserEmail();
        return Ok(await userService.UpdateUserAsync(dto, email));
    }

    [HttpGet("profile")]
    public async Task<ActionResult<UserProfileDto>> GetUserProfile()
    {
        var email = HttpContext.GetUserEmail();
        return Ok(await userService.GetProfileAsync(email));
    }

    [HttpGet("profile/{id:long}")]
    public async Task<ActionResult<UserProfileDto>> GetUserProfileById(long id) =>
        Ok(await userService.GetProfileByIdAsync(id));

    [HttpGet("friends")]
    public async Task<ActionResult<List<UserDto>>> GetUserFriends()
    {
        var email = HttpContext.GetUserEmail();
        return Ok(await userService.GetUserFriendsAsync(email));
    }
}
