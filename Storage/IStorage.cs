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
    Task AddJob(Job job);
    
    /// <summary>
    /// Returns jobs to be processed by the thread pool.
    /// </summary>
    Task<IReadOnlyList<Execution>> JobsToRun();
    
    /// <summary>
    /// Updates state of the execution under a certain key.
    /// </summary>
    Task UpdateExecutionState(int executionId, ExecutionState newState);
    
    /// <summary>
    /// Updates an execution's state to a failed. 
    /// </summary>
    Task SetExecutionFailedState(int executionId, string errorMessage);
    
    /// <summary>
    /// Returns all executions.
    /// </summary>
    Task<List<Execution>> ExecutionsPage(int pageIndex);

    /// <summary>
    /// Returns time span for the next nearest job to be executed.
    /// </summary>
    Task<TimeSpan> NearestExecutionTimeForJob();

    /// <summary>
    /// All jobs registered by user in the code.
    /// </summary>
    Task<List<Job>> AllJobs();

    /// <summary>
    /// Returns executions for the job id.
    /// </summary>
    Task<Execution?> GetExecution(int jobId);

    /// <summary>
    /// Returns execution by id.
    /// </summary>
    Task<Execution?> ExecutionById(int id);

    /// <summary>
    /// Returns total number of pages for executions.
    /// </summary>
    Task<int> TotalExecutionPages();

    /// <summary>
    /// Returns all not ended executions -- that should be re-run on the threadpool.
    /// </summary>
    Task<IReadOnlyList<Execution>> NotEndedExecutions();
}