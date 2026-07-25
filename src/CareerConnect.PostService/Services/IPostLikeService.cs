namespace CareerConnect.PostService.Services;

public interface IPostLikeService
{
    Task<string> LikePostAsync(long postId, long userId);
}
