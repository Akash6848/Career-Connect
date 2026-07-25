using System.Net;
using CareerConnect.CompanyJobService.Dto;
using CareerConnect.CompanyJobService.Enums;
using CareerConnect.CompanyJobService.Services;
using CareerConnect.Shared.Auth;
using CareerConnect.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.CompanyJobService.Controllers;

[ApiController]
[Route("api/jobs")]
[Authorize]
public class JobsController(IJobService jobService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<JobDto>> CreateJob([FromBody] CreateJobDto dto)
    {
        var ownerId = HttpContext.GetUserId();
        var created = await jobService.CreateJobAsync(dto, ownerId);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpGet]
    public async Task<ActionResult<List<JobDto>>> GetAllJobs() => Ok(await jobService.GetAllJobsAsync());

    [HttpGet("{id:long}")]
    public async Task<ActionResult<JobDto>> GetJobById(long id) => Ok(await jobService.GetJobByIdAsync(id));

    [HttpGet("detailed/{id:long}")]
    public async Task<ActionResult<JobDetailedDto>> GetJobDetailed(long id) => Ok(await jobService.GetJobDetailedAsync(id));

    [HttpGet("company/{id:long}")]
    public async Task<ActionResult<List<JobDto>>> GetJobsByCompany(long id) => Ok(await jobService.GetJobsByCompanyAsync(id));

    [HttpGet("category/{id:long}")]
    public async Task<ActionResult<List<JobDto>>> GetJobsByCategory(long id) => Ok(await jobService.GetJobsByCategoryAsync(id));

    [HttpGet("sorted/{sortType}")]
    public async Task<ActionResult<List<JobDto>>> GetJobsSorted(string sortType)
    {
        var direction = sortType.ToLowerInvariant() switch
        {
            "asc" or "ascending" => SortDirection.Ascending,
            "desc" or "descending" => SortDirection.Descending,
            _ => throw new ApiException(HttpStatusCode.BadRequest, "sortType must be 'ascending' or 'descending'")
        };

        return Ok(await jobService.GetJobsSortedAsync(direction));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<JobDto>> UpdateJob(long id, [FromBody] UpdateJobDto dto)
    {
        var ownerId = HttpContext.GetUserId();
        var isAdmin = HttpContext.IsAdmin();
        return Ok(await jobService.UpdateJobAsync(id, dto, ownerId, isAdmin));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteJob(long id)
    {
        var ownerId = HttpContext.GetUserId();
        var isAdmin = HttpContext.IsAdmin();
        await jobService.DeleteJobAsync(id, ownerId, isAdmin);
        return Ok();
    }

    [HttpPost("applied")]
    public async Task<ActionResult<AppliedJobDto>> ApplyToJob([FromBody] CreateAppliedJobDto dto)
    {
        var userId = HttpContext.GetUserId();
        var application = await jobService.ApplyToJobAsync(dto, userId);
        return StatusCode(StatusCodes.Status201Created, application);
    }

    [HttpGet("applied/{id:long}")]
    public async Task<ActionResult<List<AppliedJobDto>>> GetApplicationsForJob(long id) =>
        Ok(await jobService.GetApplicationsForJobAsync(id));
}
