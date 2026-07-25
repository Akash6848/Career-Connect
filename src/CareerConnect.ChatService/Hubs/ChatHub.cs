using CareerConnect.ChatService.Data;
using CareerConnect.Shared.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.ChatService.Hubs;

/// <summary>
/// Real-time delivery channel for chat. Clients connect once; on connect they are joined to a
/// SignalR group per chat they participate in. The REST endpoints in ChatsController persist
/// messages and then push them out via IHubContext&lt;ChatHub&gt; to everyone in the chat's group.
/// </summary>
[Authorize]
public class ChatHub(ChatDbContext db) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()!.GetUserId();

        var chatIds = await db.ChatParticipants
            .Where(p => p.UserId == userId)
            .Select(p => p.ChatId)
            .ToListAsync();

        foreach (var chatId in chatIds)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(chatId));
        }

        await base.OnConnectedAsync();
    }

    public static string GroupName(long chatId) => $"chat-{chatId}";
}
