using System.ComponentModel.DataAnnotations;

namespace CareerConnect.ChatService.Dto;

public class CreateChatDto
{
    /// <summary>The other participant(s) - the caller is added automatically.</summary>
    [Required, MinLength(1)]
    public List<long> ParticipantIds { get; set; } = [];
}

public class ChatDto
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<long> ParticipantIds { get; set; } = [];
    public ChatMessageDto? LastMessage { get; set; }
}
