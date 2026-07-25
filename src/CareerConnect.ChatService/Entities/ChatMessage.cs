namespace CareerConnect.ChatService.Entities;

public class ChatMessage
{
    public long Id { get; set; }

    public long ChatId { get; set; }
    public Chat Chat { get; set; } = null!;

    public long SenderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public DateTime? EditedAt { get; set; }
}
