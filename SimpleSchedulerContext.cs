using Microsoft.EntityFrameworkCore;
using SimpleScheduler.Entities;

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
        modelBuilder.Entity<Execution>();
        modelBuilder.Entity<Job>();
    }
}