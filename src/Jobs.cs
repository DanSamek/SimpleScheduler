using System.Text.Json.Serialization;

namespace SimpleScheduler;

/// <summary>
/// Class for adding jobs to the storage.
/// </summary>
public static class Jobs
{
    private static IStorage? _storage;
    private static IJobMapper? _jobMapper;

    /// <summary>
    /// Sets a storage to use for jobs.
    /// </summary>
    public static void SetStorage(IStorage storage) => _storage = storage;
    
    /// <summary>
    /// Sets a job mapper to use.
    /// </summary>
    public static void SetJobMapper(IJobMapper mapper) => _jobMapper = mapper;

    /// <summary>
    /// Executes a job once.
    /// </summary>
    public static void AddInstantJob(Func<Task> job, TimeSpan? delay = null)
    {
        ArgumentNullException.ThrowIfNull(_storage, "Storage is null");
        ArgumentNullException.ThrowIfNull(_jobMapper, "JobMapper is null");
        var jobKey = job.Key();
        _jobMapper.AddJob(job, jobKey);
        _storage.AddJob(new Job(jobKey, delay));
    }

    /// <summary>
    /// Executes a job repeatedly.
    /// </summary>
    public static void AddRecurringJob(Func<Task> job, TimeSpan recurrence, TimeSpan? delay = null)
    {
        ArgumentNullException.ThrowIfNull(_storage, "Storage is null");
        ArgumentNullException.ThrowIfNull(_jobMapper, "JobMapper is null");
        var jobKey = job.Key();
        _jobMapper.AddJob(job, jobKey);
        _storage.AddJob(new Job(jobKey, delay, recurrence));
    }
}

public static class JobsExtensions
{
    public static string Key(this Func<Task> job)
    {
        var key = $"{job.Target?.GetType().FullName} {job.Method.Name}";
        return key;
    }
}
