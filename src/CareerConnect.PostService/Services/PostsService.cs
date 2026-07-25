using System.Net;
using CareerConnect.PostService.Data;
using CareerConnect.PostService.Dto;
using CareerConnect.PostService.Entities;
using CareerConnect.PostService.Enums;
using CareerConnect.Shared.Clients;
using CareerConnect.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.PostService.Services;

public class PostsService(PostsDbContext db, IFileServiceClient fileServiceClient) : IPostsService
{
    public async Task<PostDto> CreatePostAsync(CreatePostDto dto, long userId)
    {
        if (await db.Posts.AnyAsync(p => p.Title == dto.Title))
        {
            throw new ApiException(HttpStatusCode.BadRequest, "a post with this title already exists");
        }

        var post = new Post
        {
            PostedById = userId,
            Title = dto.Title,
            Description = dto.Description,
            PostedAt = DateTime.UtcNow
        };

        db.Posts.Add(post);
        await db.SaveChangesAsync();

        return ToPostDto(post, likedByUserId: false, numComments: 0, numLikes: 0, link: null, comments: []);
    }

    public async Task<string> UploadPostFileAsync(IFormFile file, string fileType, long postId, long userId)
    {
        if (!Enum.TryParse<PostFileType>(fileType, ignoreCase: true, out var postFileType))
        {
            throw new ApiException(HttpStatusCode.BadRequest,
                $"Invalid File Type. Only {PostFileType.Image}, {PostFileType.Video} are supported");
        }

        var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == postId)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"post with id {postId} was not found");

        if (post.PostedById != userId)
        {
            throw new ApiException(HttpStatusCode.Forbidden, "You cannot attach a file to someone else's post");
        }

        string url;
        try
        {
            await using var stream = file.OpenReadStream();
            url = await fileServiceClient.UploadFileAsync(new Refit.StreamPart(stream, file.FileName, file.ContentType));
        }
        catch (Exception)
        {
            throw new ApiException(HttpStatusCode.InternalServerError, "Error while uploading file");
        }

        var existing = await db.PostFiles.FirstOrDefaultAsync(f => f.PostId == postId);
        if (existing is not null)
        {
            existing.Type = postFileType;
            existing.Link = url;
        }
        else
        {
            db.PostFiles.Add(new PostFiles { PostId = postId, Type = postFileType, Link = url });
        }

        await db.SaveChangesAsync();

        return url;
    }

    public async Task<List<PostDto>> GetAllPostsAsync(long userId)
    {
        var posts = await db.Posts
            .Include(p => p.PostFile)
            .OrderByDescending(p => p.PostedAt)
            .ToListAsync();

        var postIds = posts.Select(p => p.Id).ToList();

        var commentCounts = await db.PostComments
            .Where(c => postIds.Contains(c.PostId))
            .GroupBy(c => c.PostId)
            .Select(g => new { PostId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PostId, x => x.Count);

        var likeCounts = await db.PostLikes
            .Where(l => postIds.Contains(l.PostId))
            .GroupBy(l => l.PostId)
            .Select(g => new { PostId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.PostId, x => x.Count);

        var likedPostIds = await db.PostLikes
            .Where(l => postIds.Contains(l.PostId) && l.UserId == userId)
            .Select(l => l.PostId)
            .ToListAsync();
        var likedSet = likedPostIds.ToHashSet();

        return posts.Select(post => ToPostDto(
            post,
            likedByUserId: likedSet.Contains(post.Id),
            numComments: commentCounts.GetValueOrDefault(post.Id),
            numLikes: likeCounts.GetValueOrDefault(post.Id),
            link: post.PostFile?.Link,
            comments: [])).ToList();
    }

    public async Task<PostDto> GetPostByIdAsync(long id, long userId)
    {
        var post = await db.Posts
            .Include(p => p.PostFile)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"post with id {id} was not found");

        var numComments = await db.PostComments.CountAsync(c => c.PostId == id);
        var numLikes = await db.PostLikes.CountAsync(l => l.PostId == id);
        var isLiked = await db.PostLikes.AnyAsync(l => l.PostId == id && l.UserId == userId);

        var topLevelComments = await db.PostComments
            .Where(c => c.PostId == id && c.ParentId == null)
            .OrderByDescending(c => c.PostedAt)
            .Select(c => new Dto.Comments.PostCommentDto
            {
                Id = c.Id,
                Post = c.PostId,
                User = c.UserId,
                Parent = c.ParentId,
                Text = c.Text,
                PostedAt = c.PostedAt
            })
            .ToListAsync();

        return ToPostDto(post, isLiked, numComments, numLikes, post.PostFile?.Link, topLevelComments);
    }

    public async Task<string> DeletePostByIdAsync(long id, long userId, bool isAdmin)
    {
        var post = await db.Posts.FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new ApiException(HttpStatusCode.NotFound, $"post with id {id} was not found");

        if (post.PostedById != userId && !isAdmin)
        {
            throw new ApiException(HttpStatusCode.Forbidden, "You cannot delete someone else's post");
        }

        db.Posts.Remove(post);
        await db.SaveChangesAsync();

        return "Post deleted successfully";
    }

    private static PostDto ToPostDto(
        Post post,
        bool likedByUserId,
        int numComments,
        int numLikes,
        string? link,
        List<Dto.Comments.PostCommentDto> comments) => new()
    {
        Id = post.Id,
        PostedBy = post.PostedById,
        Title = post.Title,
        Description = post.Description,
        PostedAt = post.PostedAt,
        NumComments = numComments,
        NumLikes = numLikes,
        IsLiked = likedByUserId,
        Comments = comments,
        Link = link
    };
}
