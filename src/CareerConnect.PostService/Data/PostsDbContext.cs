using CareerConnect.PostService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.PostService.Data;

public class PostsDbContext(DbContextOptions<PostsDbContext> options) : DbContext(options)
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostComment> PostComments => Set<PostComment>();
    public DbSet<PostFiles> PostFiles => Set<PostFiles>();
    public DbSet<PostLikes> PostLikes => Set<PostLikes>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasIndex(p => p.Title).IsUnique();
            entity.Property(p => p.Title).HasMaxLength(100);

            entity.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.Likes)
                .WithOne(l => l.Post)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.PostFile)
                .WithOne(f => f.Post)
                .HasForeignKey<PostFiles>(f => f.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PostComment>(entity =>
        {
            entity.HasOne(c => c.Parent)
                .WithMany()
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<PostFiles>(entity =>
        {
            entity.HasIndex(f => new { f.PostId, f.Type }).IsUnique();
        });

        modelBuilder.Entity<PostLikes>(entity =>
        {
            entity.HasIndex(l => new { l.PostId, l.UserId }).IsUnique();
        });
    }
}
