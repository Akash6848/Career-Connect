using System.ComponentModel.DataAnnotations;
using CareerConnect.PostService.Dto.Comments;

namespace CareerConnect.PostService.Dto;

public class CreatePostDto
{
    [Required, StringLength(100, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(10)]
    public string Description { get; set; } = string.Empty;
}

public class PostDto
{
    public long Id { get; set; }
    public long PostedBy { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PostedAt { get; set; }
    public int NumComments { get; set; }
    public int NumLikes { get; set; }
    public bool IsLiked { get; set; }
    public List<PostCommentDto> Comments { get; set; } = [];
    public string? Link { get; set; }
}
