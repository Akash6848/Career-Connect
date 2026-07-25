using CareerConnect.ChatService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.ChatService.Data;

public class ChatDbContext(DbContextOptions<ChatDbContext> options) : DbContext(options)
{
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<ChatParticipant> ChatParticipants => Set<ChatParticipant>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasMany(c => c.Participants)
                .WithOne(p => p.Chat)
                .HasForeignKey(p => p.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Messages)
                .WithOne(m => m.Chat)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ChatParticipant>(entity =>
        {
            entity.HasIndex(p => new { p.ChatId, p.UserId }).IsUnique();
        });
    }
}
