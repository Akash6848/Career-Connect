using CareerConnect.Shared.Auth;
using CareerConnect.UsersService.Dto.Experience;
using CareerConnect.UsersService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.UsersService.Controllers;

[ApiController]
[Route("api/users/experiences")]
[Authorize]
public class ExperiencesController(IExperienceService experienceService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateExperience([FromBody] CreateExperienceDto dto)
    {
        var userId = HttpContext.GetUserId();
        await experienceService.CreateExperienceAsync(dto, userId);
        return Ok();
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateExperience([FromBody] UpdateExperienceDto dto, long id)
    {
        var userId = HttpContext.GetUserId();
        await experienceService.UpdateExperienceAsync(dto, id, userId);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<List<ExperienceDto>>> GetAllExperiences()
    {
        var userId = HttpContext.GetUserId();
        return Ok(await experienceService.GetAllExperiencesByUserIdAsync(userId));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ExperienceDto>> GetExperienceById(long id) =>
        Ok(await experienceService.GetExperienceByIdAsync(id));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteExperienceById(long id)
    {
        var isAdmin = HttpContext.IsAdmin();
        var userId = HttpContext.GetUserId();
        await experienceService.DeleteExperienceByIdAsync(id, userId, isAdmin);
        return Ok();
    }
}
