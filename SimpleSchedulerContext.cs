using Microsoft.EntityFrameworkCore;
using SimpleScheduler.Entities.Db;

namespace SimpleScheduler;

public class SimpleSchedulerContext : DbContext
{
    /// <summary>
    /// Ctor
    /// </summary>
    public SimpleSchedulerContext(DbContextOptions<SimpleSchedulerContext> options)
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddSimpleScheduler();
    }
}

public static class ModelBuilderExtensions
{
    public static void AddSimpleScheduler(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Execution>()
            .HasMany(e => e.Errors)
            .WithOne(e => e.Execution)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Job>()
            .HasOne(j => j.JobInfo)
            .WithOne(j => j.Job)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Job>()
            .HasOne(j => j.JobSettings)
            .WithOne(j => j.Job)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Token>();
        modelBuilder.Entity<Error>();
    }
}