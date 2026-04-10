using System.Globalization;
using SimpleScheduler.Entities.Dto;

namespace SimpleScheduler.Entities.Db;

internal  class Execution : DoId, IDto<ExecutionDto>
{
    /// <summary>
    /// Job that is executed.
    /// </summary>
    public Job? Job { get; set; }
    
    /// <summary>
    /// Id of the job that is executed. 
    /// </summary>
    public int JobId { get; set; }
    
    /// <summary>
    /// State of the job execution.
    /// </summary>
    public ExecutionState State { get; set; } = ExecutionState.Created;
    

    /// <summary>
    /// When the execution was created. 
    /// </summary>
    public DateTime Created { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Time when job was started.
    /// </summary>
    public DateTime? Started { get; set; }
    
    /// <summary>
    /// Time when job ended.
    /// </summary>
    public DateTime? Ended { get; set; } 
    
    /// <summary>
    /// How many times was retried to run a job.
    /// </summary>
    public int RetryCount { get; set; }
    
    /// <summary>
    /// Maximum number of retry tries.
    /// </summary>
    public int MaxRetryCount { get; set; }
    
    /// <summary>
    /// Rerun time of the execution - when its retried, can be delayed.
    /// </summary>
    public DateTime RetryTime { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// All errors that happened with this execution.
    /// </summary>
    public List<Error> Errors { get; set; } = [];
    
    /// <inheritdoc /> 
    public ExecutionDto? ToDto(int recursionDepth)
    {
        if (recursionDepth == 0) return null;
        
        const string NOT_STARTED = "Not started";
        const string NOT_ENDED = "Not ended";
        
        return new ExecutionDto(Id, Job?.JobInfo.Key, State.ToString(),
            Started?.ToString(CultureInfo.InvariantCulture) ?? NOT_STARTED,
            Ended?.ToString(CultureInfo.InvariantCulture) ?? NOT_ENDED, "TODO", RetryCount,
            Job?.ToDto(recursionDepth - 1));
    }
}