using System.Globalization;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Entities;

public class Execution : DoId, IDto<ExecutionDto>
{
    /// <summary>
    /// Job that is executed.
    /// </summary>
    public required Job Job { get; set; }
    
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
    public DateTime Started { get; set; }
    
    /// <summary>
    /// Time when job ended.
    /// </summary>
    public DateTime Ended { get; set; } 
    
    /// <inheritdoc /> 
    public ExecutionDto ToDto()
    {
        return new ExecutionDto(Id, Job.Key, State.ToString(),
            Started.ToString(CultureInfo.InvariantCulture), Ended.ToString(CultureInfo.InvariantCulture) ?? "", Error);
    }
}