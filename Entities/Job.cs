using System.Text.Json;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Entities;

public class Job : DoId, IDto<JobDto>
{
    /// <summary>
    /// Specific info about the job.
    /// </summary>
    public required JobInfo JobInfo { get; set; }
    
    /// <summary>
    /// Settings for the job. 
    /// </summary>
    public required JobSettings JobSettings { get; set; }
    
    /// <summary>
    /// Next execution time of the job.
    /// </summary>
    public DateTime NextExecutionTime { get; set; }

    /// <summary>
    /// List of all executions for the job.
    /// </summary>
    public List<Execution> Executions { get; set; } = [];
    
    public JobDto? ToDto(int recursionDepth)
    {
        throw new NotImplementedException();
    }
    
    internal void MoveExecutionTime()
    {
        // TODO
        /*if (Recurrence.HasValue)
        {
            NextExecutionTime = NextExecutionTime.Add(Recurrence.Value);
        }*/
    }
    
    /// <summary>
    /// If the job is recurrent.
    /// </summary>
    private bool IsRecurrent() => false;
}
