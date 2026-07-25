namespace CareerConnect.ChatService.Entities;

public class ChatParticipant
{
    public long Id { get; set; }

    public long ChatId { get; set; }
    public Chat Chat { get; set; } = null!;

    /// <summary>The UsersService user id of a chat participant.</summary>
    public long UserId { get; set; }
}
