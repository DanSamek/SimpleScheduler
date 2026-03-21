using SimpleScheduler.Entities;

namespace SimpleScheduler.Mapper;

/// <summary>
/// Interface for the job mapper.
/// It's a in-memory cache for jobs.
/// </summary>
public interface IJobMapper
{
    /// <summary>
    /// Adds a job the mapper.
    /// </summary>
    void AddJob(Func<Task> job, string key);
    
    /// <summary>
    /// Returns mapped jobs to the specified keys.  
    /// </summary>
    IEnumerable<Func<Task>> GetTaskForJobs(IEnumerable<Job> jobs);
}