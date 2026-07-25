namespace CareerConnect.PostService.Entities;

public class Post
{
    public long Id { get; set; }

    /// <summary>
    /// User identity lives solely in CareerConnect.UsersService (carried by the JWT), so this
    /// service stores only the author's id rather than duplicating user data locally.
    /// </summary>
    public long PostedById { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PostedAt { get; set; }

    public List<PostComment> Comments { get; set; } = [];
    public List<PostLikes> Likes { get; set; } = [];
    public PostFiles? PostFile { get; set; }
}
