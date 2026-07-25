using CareerConnect.PostService.Dto.Likes;
using CareerConnect.PostService.Services;
using CareerConnect.Shared.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.PostService.Controllers;

[ApiController]
[Route("api/posts/likes")]
[Authorize]
public class PostLikesController(IPostLikeService postLikeService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> LikePost([FromBody] CreateLikeDto dto)
    {
        var userId = HttpContext.GetUserId();
        var result = await postLikeService.LikePostAsync(dto.PostId, userId);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
