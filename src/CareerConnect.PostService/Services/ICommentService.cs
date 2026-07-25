using CareerConnect.PostService.Dto.Comments;

namespace CareerConnect.PostService.Services;

public interface ICommentService
{
    Task<PostCommentDto> AddPostCommentAsync(CreatePostCommentDto dto, long userId);
    Task<PostCommentDto> GetCommentByIdAsync(long id);
    Task<List<PostCommentDto>> GetReplyCommentsAsync(long id);
    Task<List<PostCommentDto>> GetPostCommentsAsync(long id);
    Task<string> DeleteCommentByIdAsync(long id, bool isAdmin, long userId);
}
