using SimpleScheduler.Entities;
using SimpleScheduler.Entities.Db;

namespace SimpleScheduler.Services;

/// <summary>
/// Interface for different kinds of storage implementations (in-memory, db)
/// </summary>
public interface IStorage
{
    /// <summary>
    /// Add job to the storage.
    /// </summary>  
    Task<Job> AddJob(Job job);
    
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
    Task SetExecutionFailedState(int executionId);
    
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
    Task<Execution?> GetExecutionByJobId(int jobId);

    /// <summary>
    /// Returns execution by id.
    /// </summary>
    Task<Execution?> ExecutionById(int id);

    /// <summary>
    /// Returns total number of pages for executions.
    /// </summary>
    Task<int> TotalExecutionPages();
    
    /// <summary>
    /// Returns total number of executions.
    /// </summary>
    Task<int> TotalExecutions();

    /// <summary>
    /// Returns all not ended executions -- that should be re-run on the threadpool.
    /// </summary>
    Task<IReadOnlyList<Execution>> NotEndedExecutions();

    /// <summary>
    /// Returns job by an id.
    /// </summary>
    Task<Job?> JobById(int id);

    /// <summary>
    /// Returns first <see cref="Constants.PAGE_SIZE"/> executions with requested state.
    /// </summary>
    Task<List<Execution>> GetExecutionsByState(ExecutionState state);

    /// <summary>
    /// Returns job by id with loaded parameters.
    /// </summary>
    Task<Job?> GetJobById(int id);

    /// <summary>
    /// If execution can be retried.
    /// </summary>
    Task<bool> CanBeRetried(int executionId);

    /// <summary>
    /// Updates state of the execution -- to be again scheduled. 
    /// </summary>
    Task RetryExecution(int executionId);

    /// <summary>
    /// Returns all executions to retry.
    /// </summary>
    Task<List<Execution>> ExecutionsToRetry();

    /// <summary>
    /// Returns all errors that occured.
    /// </summary>
    Task<IReadOnlyList<Error>> AllErrors();

    /// <summary>
    /// Returns error by id.
    /// </summary>
    Task<Error?> ErrorById(int id);
}