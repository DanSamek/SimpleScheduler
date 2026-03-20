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
    public IReadOnlyList<string> JobsKeysToRun();
    
    /// <summary>
    /// Updates state of the job under certain key.
    /// </summary>
    public Task UpdateJobState(string jobKey, JobState newState);

    /// <summary>
    /// Sets properly state for the task:
    /// if its not recurrent -> Ended
    /// if its -> Inactive
    /// </summary>
    void SetEndedState(string jobKey);
}