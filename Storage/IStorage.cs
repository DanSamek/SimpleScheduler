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
    /// Returns jobs to be processed by the thread pool.
    /// </summary>
    public IReadOnlyList<Execution> JobsToRun();
    
    /// <summary>
    /// Updates state of the execution under a certain key.
    /// </summary>
    public Task UpdateExecutionState(int executionId, ExecutionState newState);
    
    /// <summary>
    /// Updates an execution's state to a failed. 
    /// </summary>
    public Task SetExecutionFailedState(int executionId, string errorMessage);
    
    /// <summary>
    /// Returns all executions.
    /// </summary>
    public List<Execution> AllExecutions();
}