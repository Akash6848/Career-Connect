using CareerConnect.ChatService.Dto;

namespace CareerConnect.ChatService.Services;

public interface IChatsService
{
    Task<ChatDto> CreateChatAsync(CreateChatDto dto, long userId);
    Task<List<ChatDto>> GetChatsForUserAsync(long userId);
    Task<ChatDto> GetChatByIdAsync(long id, long userId);
    Task DeleteChatAsync(long id, long userId);

    Task<ChatMessageDto> SendMessageAsync(SendMessageDto dto, long userId);
    Task<ChatMessageDto> UpdateMessageAsync(long id, UpdateMessageDto dto, long userId);
    Task DeleteMessageAsync(long id, long userId);
}
