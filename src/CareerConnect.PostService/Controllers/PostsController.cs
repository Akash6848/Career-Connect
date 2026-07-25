using CareerConnect.PostService.Dto;
using CareerConnect.PostService.Services;
using CareerConnect.Shared.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.PostService.Controllers;

[ApiController]
[Route("api/posts")]
[Authorize]
public class PostsController(IPostsService postsService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostDto dto)
    {
        var userId = HttpContext.GetUserId();
        var created = await postsService.CreatePostAsync(dto, userId);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPost("upload")]
    public async Task<ActionResult<string>> UploadPostFile(IFormFile file, [FromForm] string type, [FromForm] long postId)
    {
        var userId = HttpContext.GetUserId();
        var url = await postsService.UploadPostFileAsync(file, type, postId, userId);
        return Ok(url);
    }

    [HttpGet]
    public async Task<ActionResult<List<PostDto>>> GetAllPosts()
    {
        var userId = HttpContext.GetUserId();
        return Ok(await postsService.GetAllPostsAsync(userId));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PostDto>> GetPostById(long id)
    {
        var userId = HttpContext.GetUserId();
        return Ok(await postsService.GetPostByIdAsync(id, userId));
    }

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<string>> DeletePostById(long id)
    {
        var isAdmin = HttpContext.IsAdmin();
        var userId = HttpContext.GetUserId();
        return Ok(await postsService.DeletePostByIdAsync(id, userId, isAdmin));
    }
}
