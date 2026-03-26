namespace SimpleScheduler;

/// <summary>
/// Configuration for the scheduler.
/// </summary>
public record SimpleSchedulerOptions
{
    /// <summary>
    /// Number of threads that can be used for the thread pool.
    /// </summary>
    public int NumberOfThreads { get; set; }

    /// <summary>
    /// Type of the EF Core DbContext that the application is using.
    /// </summary>
    public Type? DbContextType { get; set; } 

    /// <summary>
    /// User credentials to login into the scheduler sites.
    /// </summary>
    public SimpleSchedulerUser? User { get; set; }
    
    /// <summary>
    /// Validates options. 
    /// </summary>
    internal void Validate()
    {
        if (NumberOfThreads < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(NumberOfThreads), $"{nameof(NumberOfThreads)} must be greater than 0");
        }

        if (DbContextType == null)
        {
            throw new ArgumentNullException(nameof(DbContextType));
        }

        if (User == null)
        {
            throw new ArgumentNullException(nameof(User));
        }
    }
}