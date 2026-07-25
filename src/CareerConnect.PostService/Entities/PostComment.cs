namespace CareerConnect.PostService.Entities;

public class PostComment
{
    public long Id { get; set; }

    public long PostId { get; set; }
    public Post Post { get; set; } = null!;

    public long UserId { get; set; }

    public long? ParentId { get; set; }
    public PostComment? Parent { get; set; }

    public string Text { get; set; } = string.Empty;
    public DateTime PostedAt { get; set; }
}
