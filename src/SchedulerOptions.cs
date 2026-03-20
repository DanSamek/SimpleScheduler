namespace SimpleScheduler;

/// <summary>
/// Configuration for the scheduler.
/// </summary>
public record SchedulerOptions
{
    /// <summary>
    /// Number of threads that can be used for the thread pool.
    /// </summary>
    public int NumberOfThreads { get; set; }
}