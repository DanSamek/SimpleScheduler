using System.Linq.Expressions;
using System.Text.Json;
using SimpleScheduler.Entities;
using SimpleScheduler.Scheduler;
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
    public static void SetStorage(IStorage storage) => _storage = storage;
    
    /// <summary>
    /// Executes a job once.
    /// </summary>
    public static async Task AddInstantJob(UserJobInfo userJobInfo, UserJobSettings userJobSettings)
    {
        var job = new Job
        {
            JobInfo = new JobInfo(userJobInfo),
            JobSettings = new JobSettings(userJobSettings)
        };
        
        // Move execution time for the job with delay value.
        if (userJobSettings.Delay.HasValue)
        {
            job.NextExecutionTime = job.NextExecutionTime.Add(userJobSettings.Delay.Value);
        }
        
        await _storage.AddJob(job);
    }
}

