namespace SimpleScheduler;

/// <summary>
/// Class for adding jobs to the storage.
/// </summary>
public static class Jobs
{
    public static IStorage? Storage;

    /// <summary>
    /// Executes a job once.
    /// </summary>
    public static void AddInstantJob(Func<Task> job, TimeSpan? delay = null)
    {
        ArgumentNullException.ThrowIfNull(Storage, "Storage is null");
        Storage.AddJob(new Job(job, Delay: delay));
    }

    /// <summary>
    /// Executes a job repeatedly.
    /// </summary>
    public static void AddRecurringJob(Func<Task> job, TimeSpan interval, TimeSpan? delay = null)
    {
        ArgumentNullException.ThrowIfNull(Storage, "Storage is null");
        Storage.AddJob(new Job(job, interval, delay));
    }
}