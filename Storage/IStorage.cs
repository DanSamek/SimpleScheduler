using SimpleScheduler.Entities;

namespace SimpleScheduler.Storage;

/// <summary>
/// Interface for different kinds of storage implementations (in-memory, db)
/// </summary>
public interface IStorage
{
    /// <summary>
    /// Add job to the storage.
    /// </summary>
    void AddJob(Job job);
    
    /// <summary>
    /// Returns jobs keys to be processed by the thread pool.
    /// </summary>
    public IReadOnlyList<Job> JobsToRun();
    
    /// <summary>
    /// Updates state of the job under certain key.
    /// </summary>
    public Task UpdateJobState(string jobKey, JobState newState);
    
    /// <summary>
    /// Updates a job state to a failed. 
    /// </summary>
    public Task SetFailedState(string jobKey, string errorMessage);

    /// <summary>
    /// Sets properly state for the task:
    /// if its not recurrent -> Ended
    /// if its -> Inactive
    /// </summary>
    public Task SetEndedState(string jobKey);
    
    /// <summary>
    /// Returns all jobs.
    /// </summary>
    public List<Job> AllJobs();
}