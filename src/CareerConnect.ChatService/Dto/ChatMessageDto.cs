using System.ComponentModel.DataAnnotations;

namespace CareerConnect.ChatService.Dto;

public class SendMessageDto
{
    [Required]
    public long ChatId { get; set; }

    [Required, StringLength(2000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
}

public class UpdateMessageDto
{
    [Required, StringLength(2000, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
}

public class ChatMessageDto
{
    public long Id { get; set; }
    public long ChatId { get; set; }
    public long SenderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public DateTime? EditedAt { get; set; }
}
