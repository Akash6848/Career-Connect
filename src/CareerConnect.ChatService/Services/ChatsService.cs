using System.Net;
using CareerConnect.ChatService.Data;
using CareerConnect.ChatService.Dto;
using CareerConnect.ChatService.Entities;
using CareerConnect.ChatService.Hubs;
using CareerConnect.Shared.Exceptions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.ChatService.Services;

public class ChatsService(ChatDbContext db, IHubContext<ChatHub> hubContext) : IChatsService
{
    public async Task<ChatDto> CreateChatAsync(CreateChatDto dto, long userId)
    {
        var participantIds = dto.ParticipantIds.Append(userId).Distinct().ToList();

        var chat = new Chat
        {
            CreatedAt = DateTime.UtcNow,
            Participants = participantIds.Select(id => new ChatParticipant { UserId = id }).ToList()
        };

        db.Chats.Add(chat);
        await db.SaveChangesAsync();

        return ToChatDto(chat, lastMessage: null);
    }

    public async Task<List<ChatDto>> GetChatsForUserAsync(long userId)
    {
        var chatIds = await db.ChatParticipants
            .Where(p => p.UserId == userId)
            .Select(p => p.ChatId)
            .ToListAsync();

        var chats = await db.Chats
            .Include(c => c.Participants)
            .Where(c => chatIds.Contains(c.Id))
            .ToListAsync();

        // Ids are monotonically increasing, so max id per chat is the latest message - this keeps
        // the query a simple GROUP BY/MAX that translates to plain SQL.
        var lastMessageIds = await db.ChatMessages
            .Where(m => chatIds.Contains(m.ChatId))
            .GroupBy(m => m.ChatId)
            .Select(g => g.Max(m => m.Id))
            .ToListAsync();

        var lastMessages = await db.ChatMessages
            .Where(m => lastMessageIds.Contains(m.Id))
            .ToListAsync();

        var lastMessageByChat = lastMessages.ToDictionary(m => m.ChatId);

        return chats
            .Select(chat => ToChatDto(chat, lastMessageByChat.GetValueOrDefault(chat.Id)))
            .ToList();
    }

    public async Task<ChatDto> GetChatByIdAsync(long id, long userId)
    {
        var chat = await GetChatOrThrowAsync(id);
        EnsureParticipant(chat, userId);

        var lastMessage = await db.ChatMessages
            .Where(m => m.ChatId == id)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefaultAsync();

        return ToChatDto(chat, lastMessage);
    }

    public async Task DeleteChatAsync(long id, long userId)
    {
        var chat = await GetChatOrThrowAsync(id);
        EnsureParticipant(chat, userId);

        db.Chats.Remove(chat);
        await db.SaveChangesAsync();
    }

    public async Task<ChatMessageDto> SendMessageAsync(SendMessageDto dto, long userId)
    {
        var chat = await GetChatOrThrowAsync(dto.ChatId);
        EnsureParticipant(chat, userId);

        var message = new ChatMessage
        {
            ChatId = dto.ChatId,
            SenderId = userId,
            Text = dto.Text,
            SentAt = DateTime.UtcNow
        };

        db.ChatMessages.Add(message);
        await db.SaveChangesAsync();

        var messageDto = ToMessageDto(message);
        await hubContext.Clients.Group(ChatHub.GroupName(dto.ChatId)).SendAsync("MessageReceived", messageDto);

        return messageDto;
    }

    public async Task<ChatMessageDto> UpdateMessageAsync(long id, UpdateMessageDto dto, long userId)
    {
        var message = await GetMessageOrThrowAsync(id);

        if (message.SenderId != userId)
        {
            throw new ApiException(HttpStatusCode.Forbidden, "You cannot edit someone else's message");
        }

        message.Text = dto.Text;
        message.EditedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var messageDto = ToMessageDto(message);
        await hubContext.Clients.Group(ChatHub.GroupName(message.ChatId)).SendAsync("MessageUpdated", messageDto);

        return messageDto;
    }

    public async Task DeleteMessageAsync(long id, long userId)
    {
        var message = await GetMessageOrThrowAsync(id);

        if (message.SenderId != userId)
        {
            throw new ApiException(HttpStatusCode.Forbidden, "You cannot delete someone else's message");
        }

        db.ChatMessages.Remove(message);
        await db.SaveChangesAsync();

        await hubContext.Clients.Group(ChatHub.GroupName(message.ChatId)).SendAsync("MessageDeleted", message.Id);
    }

    private async Task<Chat> GetChatOrThrowAsync(long id) =>
        await db.Chats.Include(c => c.Participants).FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"chat with id {id} was not found");

    private async Task<ChatMessage> GetMessageOrThrowAsync(long id) =>
        await db.ChatMessages.FirstOrDefaultAsync(m => m.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"message with id {id} was not found");

    private static void EnsureParticipant(Chat chat, long userId)
    {
        if (chat.Participants.All(p => p.UserId != userId))
        {
            throw new ApiException(HttpStatusCode.Forbidden, "You are not a participant in this chat");
        }
    }

    private static ChatDto ToChatDto(Chat chat, ChatMessage? lastMessage) => new()
    {
        Id = chat.Id,
        CreatedAt = chat.CreatedAt,
        ParticipantIds = chat.Participants.Select(p => p.UserId).ToList(),
        LastMessage = lastMessage is null ? null : ToMessageDto(lastMessage)
    };

    private static ChatMessageDto ToMessageDto(ChatMessage message) => new()
    {
        Id = message.Id,
        ChatId = message.ChatId,
        SenderId = message.SenderId,
        Text = message.Text,
        SentAt = message.SentAt,
        EditedAt = message.EditedAt
    };
}
