using CiServer.Core.Entities;
using Microsoft.EntityFrameworkCore;
namespace CiServer.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Build> Builds { get; set; }
    public DbSet<BuildLog> BuildLogs { get; set; }
    public DbSet<Artifact> Artifacts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Builds)
            .WithOne(b => b.Project)
            .HasForeignKey(b => b.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}