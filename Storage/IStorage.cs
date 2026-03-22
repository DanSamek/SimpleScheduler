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
    public Task<IReadOnlyList<Execution>> JobsToRun();
    
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

    /// <summary>
    /// Returns time span for the next nearest job to be executed.
    /// </summary>
    public Task<TimeSpan> NearestExecutionTimeForJob();

    /// <summary>
    /// All jobs registered by user in the code.
    /// </summary>
    public Task<List<Job>> AllJobs();

    /// <summary>
    /// Returns executions for the job id.
    /// </summary>
    public Task<Execution?> GetExecution(int jobId);

    /// <summary>
    /// Returns execution by id.
    /// </summary>
    public Task<Execution?> ExecutionById(int id);
}