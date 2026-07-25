using CareerConnect.CompanyJobService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.CompanyJobService.Data;

public class CompanyJobDbContext(DbContextOptions<CompanyJobDbContext> options) : DbContext(options)
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<CompanyLocation> CompanyLocations => Set<CompanyLocation>();
    public DbSet<CompanyFiles> CompanyFiles => Set<CompanyFiles>();
    public DbSet<JobCategory> JobCategories => Set<JobCategory>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<AppliedJob> AppliedJobs => Set<AppliedJob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasMany(c => c.Locations)
                .WithOne(l => l.Company)
                .HasForeignKey(l => l.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Files)
                .WithOne(f => f.Company)
                .HasForeignKey(f => f.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Jobs)
                .WithOne(j => j.Company)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<JobCategory>(entity =>
        {
            entity.HasIndex(c => c.Name).IsUnique();

            entity.HasMany(c => c.Jobs)
                .WithOne(j => j.Category)
                .HasForeignKey(j => j.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.Property(j => j.MinSalary).HasColumnType("decimal(18,2)");
            entity.Property(j => j.MaxSalary).HasColumnType("decimal(18,2)");

            entity.HasMany(j => j.Applications)
                .WithOne(a => a.Job)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AppliedJob>(entity =>
        {
            entity.HasIndex(a => new { a.JobId, a.UserId }).IsUnique();
        });
    }
}
