using System.ComponentModel.DataAnnotations;

namespace CareerConnect.PostService.Dto.Likes;

public class CreateLikeDto
{
    [Required, Range(1, long.MaxValue)]
    public long PostId { get; set; }
}

public class PostLikeDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long PostId { get; set; }
}
