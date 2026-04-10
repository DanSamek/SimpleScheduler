namespace SimpleScheduler.Scheduler;

/// <summary>
/// Context for the job.
/// </summary>
public readonly record struct SimpleSchedulerJobContext
{
    private readonly object? _data;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public SimpleSchedulerJobContext(object? data, int retryCount)
    {
        _data = data;
        RetryCount = retryCount;
    }
    
    /// <summary>
    /// Returns data that were added for the job.
    /// </summary>
    public T? GetData<T>() => (T?)_data;
    
    /// <summary>
    /// Returns data that were added for the job without null checks.
    /// This will trigger exceptions, if data are null!
    /// </summary>
    public T GetDataNotNull<T>() => ((T?)_data)!;
    
    /// <summary>
    /// How many times the job is retried.
    /// </summary>
    public int RetryCount { get; }
}