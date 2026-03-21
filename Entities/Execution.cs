using System.Globalization;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Entities;

public class Execution : DoId
{
    /// <summary>
    /// Job that is executed.
    /// </summary>
    public required Job Job { get; set; }
    
    /// <summary>
    /// State of the job execution.
    /// </summary>
    public ExecutionState State { get; set; } = ExecutionState.Queued;
    
    /// <summary>
    /// Error message of the execution.
    /// </summary>
    public string? Error { get; set; }
    
    // TODO add Executed (datetime) 
    
    public ExecutionDto AsDto()
    {
        return new ExecutionDto(Id, Job.Key, State.ToString(),
            Job.ExecutionTime.ToString(CultureInfo.InvariantCulture));
    }
}