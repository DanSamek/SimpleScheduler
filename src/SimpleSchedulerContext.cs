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
    
    public DbSet<Job> Jobs { get; set; }
}