using System.Net;
using CareerConnect.PostService.Data;
using CareerConnect.PostService.Dto.Comments;
using CareerConnect.PostService.Entities;
using CareerConnect.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.PostService.Services;

public class CommentService(PostsDbContext db) : ICommentService
{
    public async Task<PostCommentDto> AddPostCommentAsync(CreatePostCommentDto dto, long userId)
    {
        var postExists = await db.Posts.AnyAsync(p => p.Id == dto.PostId);
        if (!postExists) throw new ApiException(HttpStatusCode.NotFound, $"post with id {dto.PostId} was not found");

        if (dto.ParentId is not null)
        {
            var parentExists = await db.PostComments.AnyAsync(c => c.Id == dto.ParentId && c.PostId == dto.PostId);
            if (!parentExists) throw new ApiException(HttpStatusCode.NotFound, $"parent comment with id {dto.ParentId} was not found on this post");
        }

        var comment = new PostComment
        {
            PostId = dto.PostId,
            UserId = userId,
            ParentId = dto.ParentId,
            Text = dto.Text,
            PostedAt = DateTime.UtcNow
        };

        db.PostComments.Add(comment);
        await db.SaveChangesAsync();

        return ToDto(comment);
    }

    public async Task<PostCommentDto> GetCommentByIdAsync(long id) => ToDto(await GetOrThrowAsync(id));

    public async Task<List<PostCommentDto>> GetReplyCommentsAsync(long id) =>
        await db.PostComments
            .Where(c => c.ParentId == id)
            .OrderByDescending(c => c.PostedAt)
            .Select(c => ToDto(c))
            .ToListAsync();

    public async Task<List<PostCommentDto>> GetPostCommentsAsync(long id) =>
        await db.PostComments
            .Where(c => c.PostId == id)
            .OrderByDescending(c => c.PostedAt)
            .Select(c => ToDto(c))
            .ToListAsync();

    public async Task<string> DeleteCommentByIdAsync(long id, bool isAdmin, long userId)
    {
        var comment = await GetOrThrowAsync(id);

        if (comment.UserId != userId && !isAdmin)
        {
            throw new ApiException(HttpStatusCode.Forbidden, "You cannot delete someone else's comment");
        }

        db.PostComments.Remove(comment);
        await db.SaveChangesAsync();

        return "Comment deleted successfully";
    }

    private async Task<PostComment> GetOrThrowAsync(long id) =>
        await db.PostComments.FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"comment with id {id} was not found");

    private static PostCommentDto ToDto(PostComment comment) => new()
    {
        Id = comment.Id,
        Post = comment.PostId,
        User = comment.UserId,
        Parent = comment.ParentId,
        Text = comment.Text,
        PostedAt = comment.PostedAt
    };
}
