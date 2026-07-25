using CareerConnect.PostService.Dto.Comments;
using CareerConnect.PostService.Services;
using CareerConnect.Shared.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.PostService.Controllers;

[ApiController]
[Route("api/posts/comments")]
[Authorize]
public class CommentsController(ICommentService commentService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<PostCommentDto>> AddCommentToPost([FromBody] CreatePostCommentDto dto)
    {
        var userId = HttpContext.GetUserId();
        var comment = await commentService.AddPostCommentAsync(dto, userId);
        return StatusCode(StatusCodes.Status201Created, comment);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PostCommentDto>> GetCommentById(long id) =>
        Ok(await commentService.GetCommentByIdAsync(id));

    [HttpGet("replies/{id:long}")]
    public async Task<ActionResult<List<PostCommentDto>>> GetReplyComments(long id) =>
        Ok(await commentService.GetReplyCommentsAsync(id));

    [HttpGet("post/{id:long}")]
    public async Task<ActionResult<List<PostCommentDto>>> GetPostComments(long id) =>
        Ok(await commentService.GetPostCommentsAsync(id));

    [HttpDelete("{id:long}")]
    public async Task<ActionResult<string>> DeleteCommentById(long id)
    {
        var userId = HttpContext.GetUserId();
        var isAdmin = HttpContext.IsAdmin();
        return Ok(await commentService.DeleteCommentByIdAsync(id, isAdmin, userId));
    }
}
