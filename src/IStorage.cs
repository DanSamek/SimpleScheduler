namespace SimpleScheduler;

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
    public List<string> JobsKeysToRun();
}