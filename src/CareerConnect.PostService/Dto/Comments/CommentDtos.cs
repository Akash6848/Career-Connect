using System.ComponentModel.DataAnnotations;

namespace CareerConnect.PostService.Dto.Comments;

public class CreatePostCommentDto
{
    [Required]
    public long PostId { get; set; }

    public long? ParentId { get; set; }

    [Required, StringLength(255, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
}

public class PostCommentDto
{
    public long Id { get; set; }
    public long Post { get; set; }
    public long User { get; set; }
    public long? Parent { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime PostedAt { get; set; }
}
