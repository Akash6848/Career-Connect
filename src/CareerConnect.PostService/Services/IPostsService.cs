using CareerConnect.PostService.Dto;
using Microsoft.AspNetCore.Http;

namespace CareerConnect.PostService.Services;

public interface IPostsService
{
    Task<PostDto> CreatePostAsync(CreatePostDto dto, long userId);
    Task<string> UploadPostFileAsync(IFormFile file, string fileType, long postId, long userId);
    Task<List<PostDto>> GetAllPostsAsync(long userId);
    Task<PostDto> GetPostByIdAsync(long id, long userId);
    Task<string> DeletePostByIdAsync(long id, long userId, bool isAdmin);
}
