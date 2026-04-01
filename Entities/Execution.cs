using System.Globalization;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Entities;

public class Execution : DoId, IDto<ExecutionDto>
{
    /// <summary>
    /// Job that is executed.
    /// </summary>
    public Job? Job { get; set; }
    
    /// <summary>
    /// State of the job execution.
    /// </summary>
    public ExecutionState State { get; set; } = ExecutionState.Created;
    
    /// <summary>
    /// Error message of the execution.
    /// </summary>
    public string? Error { get; set; }

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
    
    /// <inheritdoc /> 
    public ExecutionDto? ToDto(int recursionDepth)
    {
        if (recursionDepth == 0) return null;
        
        const string NOT_STARTED = "Not started";
        const string NOT_ENDED = "Not ended";
        
        return new ExecutionDto(Id, Job?.Key, State.ToString(),
            Started?.ToString(CultureInfo.InvariantCulture) ?? NOT_STARTED, Ended?.ToString(CultureInfo.InvariantCulture) ?? NOT_ENDED, Error, RetryCount, Job?.ToDto(recursionDepth - 1));
    }
}