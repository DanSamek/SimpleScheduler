using System.Globalization;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Entities;

internal class Job : DoId, IDto<JobDto>
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
        if (recursionDepth == 0) return null;
        return new JobDto(Id, JobInfo.Key ?? $"{JobInfo.Type}.{JobInfo.MethodName}", JobInfo.Type, JobInfo.MethodName, false,
            NextExecutionTime.ToString(CultureInfo.InvariantCulture), Executions.Select(e => e.ToDto(recursionDepth - 1)).ToList());
    }
    
    internal void MoveExecutionTime()
    {
        NextExecutionTime = JobSettings.Recurrence.HasValue 
            ? NextExecutionTime.Add(JobSettings.Recurrence.Value) 
            : DateTime.MaxValue;
    }
    
    /// <summary>
    /// If the job is recurrent.
    /// </summary>
    private bool IsRecurrent() => JobSettings.Recurrence.HasValue;
}
