using CareerConnect.CompanyJobService.Dto;
using CareerConnect.CompanyJobService.Services;
using CareerConnect.Shared.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.CompanyJobService.Controllers;

[ApiController]
[Route("api/category")]
[Authorize]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<JobCategoryDto>> CreateCategory([FromBody] CreateJobCategoryDto dto)
    {
        var created = await categoryService.CreateCategoryAsync(dto);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpGet]
    public async Task<ActionResult<List<JobCategoryDto>>> GetAllCategories() =>
        Ok(await categoryService.GetAllCategoriesAsync());

    [HttpGet("{id:long}")]
    public async Task<ActionResult<JobCategoryDto>> GetCategoryById(long id) =>
        Ok(await categoryService.GetCategoryByIdAsync(id));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<JobCategoryDto>> UpdateCategory(long id, [FromBody] UpdateJobCategoryDto dto)
    {
        HttpContext.ShouldBeAdmin();
        return Ok(await categoryService.UpdateCategoryAsync(id, dto));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteCategory(long id)
    {
        HttpContext.ShouldBeAdmin();
        await categoryService.DeleteCategoryAsync(id);
        return Ok();
    }
}
