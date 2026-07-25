using CareerConnect.UsersService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.UsersService.Data;

public class UsersDbContext(DbContextOptions<UsersDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserInfo> UserInfos => Set<UserInfo>();
    public DbSet<UserFiles> UserFiles => Set<UserFiles>();
    public DbSet<Experience> Experiences => Set<Experience>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Username).HasMaxLength(30);
            entity.Property(u => u.Email).HasMaxLength(150);
            entity.Property(u => u.FirstName).HasMaxLength(20);
            entity.Property(u => u.LastName).HasMaxLength(20);

            // Self-referencing many-to-many: friends
            entity.HasMany(u => u.Friends)
                .WithMany()
                .UsingEntity(
                    "UserFriends",
                    l => l.HasOne(typeof(User)).WithMany().HasForeignKey("FriendId").HasPrincipalKey(nameof(User.Id)),
                    r => r.HasOne(typeof(User)).WithMany().HasForeignKey("UserId").HasPrincipalKey(nameof(User.Id)),
                    j => j.HasKey("UserId", "FriendId"));

            // Many-to-many: roles
            entity.HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity(j => j.ToTable("UserRoles"));

            entity.HasOne(u => u.UserInfo)
                .WithOne(ui => ui.User)
                .HasForeignKey<UserInfo>(ui => ui.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Files)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(u => u.Experiences)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(r => r.Name).IsUnique();
        });
    }
}
