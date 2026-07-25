using CareerConnect.ChatService.Dto;
using CareerConnect.ChatService.Services;
using CareerConnect.Shared.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareerConnect.ChatService.Controllers;

[ApiController]
[Route("api/chats")]
[Authorize]
public class ChatsController(IChatsService chatsService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ChatDto>> CreateChat([FromBody] CreateChatDto dto)
    {
        var userId = HttpContext.GetUserId();
        var created = await chatsService.CreateChatAsync(dto, userId);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpGet]
    public async Task<ActionResult<List<ChatDto>>> GetChats()
    {
        var userId = HttpContext.GetUserId();
        return Ok(await chatsService.GetChatsForUserAsync(userId));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ChatDto>> GetChatById(long id)
    {
        var userId = HttpContext.GetUserId();
        return Ok(await chatsService.GetChatByIdAsync(id, userId));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteChat(long id)
    {
        var userId = HttpContext.GetUserId();
        await chatsService.DeleteChatAsync(id, userId);
        return Ok();
    }

    [HttpPost("messages")]
    public async Task<ActionResult<ChatMessageDto>> SendMessage([FromBody] SendMessageDto dto)
    {
        var userId = HttpContext.GetUserId();
        var message = await chatsService.SendMessageAsync(dto, userId);
        return StatusCode(StatusCodes.Status201Created, message);
    }

    [HttpPut("messages/{id:long}")]
    public async Task<ActionResult<ChatMessageDto>> UpdateMessage(long id, [FromBody] UpdateMessageDto dto)
    {
        var userId = HttpContext.GetUserId();
        return Ok(await chatsService.UpdateMessageAsync(id, dto, userId));
    }

    [HttpDelete("messages/{id:long}")]
    public async Task<IActionResult> DeleteMessage(long id)
    {
        var userId = HttpContext.GetUserId();
        await chatsService.DeleteMessageAsync(id, userId);
        return Ok();
    }
}
