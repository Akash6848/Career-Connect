using CareerConnect.CompanyJobService.Dto;
using CareerConnect.CompanyJobService.Services;
using CareerConnect.Shared.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.CompanyJobService.Controllers;

[ApiController]
[Route("api/company")]
[Authorize]
public class CompanyController(ICompanyService companyService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompany([FromBody] CreateCompanyDto dto)
    {
        var ownerId = HttpContext.GetUserId();
        var created = await companyService.CreateCompanyAsync(dto, ownerId);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPost("upload")]
    public async Task<ActionResult<string>> UploadCompanyFile(IFormFile file, [FromForm] string fileType, [FromForm] long companyId)
    {
        var ownerId = HttpContext.GetUserId();
        var url = await companyService.UploadCompanyFileAsync(file, fileType, companyId, ownerId);
        return Ok(url);
    }

    [HttpGet]
    public async Task<ActionResult<List<CompanyDto>>> GetAllCompanies()
    {
        HttpContext.ShouldBeAdmin();
        return Ok(await companyService.GetAllCompaniesAsync());
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CompanyDto>> GetCompanyById(long id) =>
        Ok(await companyService.GetCompanyByIdAsync(id));

    [HttpGet("detailed/{id:long}")]
    public async Task<ActionResult<CompanyDetailedDto>> GetCompanyDetailed(long id) =>
        Ok(await companyService.GetCompanyDetailedAsync(id));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<CompanyDto>> UpdateCompany(long id, [FromBody] UpdateCompanyDto dto)
    {
        var ownerId = HttpContext.GetUserId();
        var isAdmin = HttpContext.IsAdmin();
        return Ok(await companyService.UpdateCompanyAsync(id, dto, ownerId, isAdmin));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteCompany(long id)
    {
        var ownerId = HttpContext.GetUserId();
        var isAdmin = HttpContext.IsAdmin();
        await companyService.DeleteCompanyAsync(id, ownerId, isAdmin);
        return Ok();
    }

    [HttpPost("locations")]
    public async Task<ActionResult<CompanyLocationDto>> AddLocation([FromBody] CreateCompanyLocationDto dto)
    {
        var ownerId = HttpContext.GetUserId();
        var created = await companyService.AddLocationAsync(dto, ownerId);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("locations/{id:long}")]
    public async Task<ActionResult<CompanyLocationDto>> UpdateLocation(long id, [FromBody] UpdateCompanyLocationDto dto)
    {
        var ownerId = HttpContext.GetUserId();
        var isAdmin = HttpContext.IsAdmin();
        return Ok(await companyService.UpdateLocationAsync(id, dto, ownerId, isAdmin));
    }

    [HttpDelete("locations/{id:long}")]
    public async Task<IActionResult> DeleteLocation(long id)
    {
        var ownerId = HttpContext.GetUserId();
        var isAdmin = HttpContext.IsAdmin();
        await companyService.DeleteLocationAsync(id, ownerId, isAdmin);
        return Ok();
    }
}
