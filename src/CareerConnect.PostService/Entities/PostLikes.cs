namespace CareerConnect.PostService.Entities;

public class PostLikes
{
    public long Id { get; set; }

    public long PostId { get; set; }
    public Post Post { get; set; } = null!;

    public long UserId { get; set; }
}
