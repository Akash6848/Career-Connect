using System.Net;
using CareerConnect.PostService.Data;
using CareerConnect.PostService.Entities;
using CareerConnect.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.PostService.Services;

/// <summary>
/// README describes this as "Like or unlike a post" via a single endpoint - so this toggles
/// the like rather than always creating one.
/// </summary>
public class PostLikeService(PostsDbContext db) : IPostLikeService
{
    public async Task<string> LikePostAsync(long postId, long userId)
    {
        var postExists = await db.Posts.AnyAsync(p => p.Id == postId);
        if (!postExists) throw new ApiException(HttpStatusCode.NotFound, $"post with id {postId} was not found");

        var existingLike = await db.PostLikes.FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

        if (existingLike is not null)
        {
            db.PostLikes.Remove(existingLike);
            await db.SaveChangesAsync();
            return "Post unliked successfully";
        }

        db.PostLikes.Add(new PostLikes { PostId = postId, UserId = userId });
        await db.SaveChangesAsync();

        return "Post liked successfully";
    }
}
