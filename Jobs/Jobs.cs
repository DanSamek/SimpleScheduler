using SimpleScheduler.Entities.Db;
using SimpleScheduler.Services;

namespace SimpleScheduler.Jobs;

/// <summary>
/// Class for adding jobs to the storage.
/// </summary>
public static class Jobs
{
    private static IStorage _storage = null!;
    
    /// <summary>
    /// Sets a storage to use for jobs.
    /// </summary>
    internal static void SetStorage(IStorage storage) => _storage = storage;
    
    /// <summary>
    /// Executes a job with the specified <see cref="UserJobInfo"/> and <see cref="UserJobSettings"/>.
    /// </summary>
    public static async Task AddJob(UserJobInfo userJobInfo, UserJobSettings userJobSettings)
    {
        var job = new Job
        {
            JobInfo = new JobInfo(userJobInfo),
            JobSettings = new JobSettings(userJobSettings)
        };
        job.NextExecutionTime = DateTime.UtcNow;
        
        // Move execution time for the job with delay value.
        if (userJobSettings.Delay.HasValue)
        {
            job.NextExecutionTime = job.NextExecutionTime.Add(userJobSettings.Delay.Value);
        }
        
        await _storage.AddJob(job);
    }
}

