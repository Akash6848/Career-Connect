namespace CareerConnect.ChatService.Entities;

public class Chat
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<ChatParticipant> Participants { get; set; } = [];
    public List<ChatMessage> Messages { get; set; } = [];
}
