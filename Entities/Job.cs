using System.Globalization;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Entities;

public class Job : DoId, IDto<JobDto>
{
    /// <summary>
    /// Ctor
    /// </summary>
    public Job(string type, string methodName, List<Argument> arguments, string? key = null, TimeSpan? recurrence = null, TimeSpan? delay = null)
    {
        Key = key ?? $"{type}.{methodName}";
        Recurrence = recurrence;
        NextExecutionTime = DateTime.UtcNow.Add(delay ?? TimeSpan.Zero);
        Type = type;
        MethodName = methodName;
        Arguments = arguments;
    }

    public Job()
    {
        Key = "";
        MethodName = "";
        Type = "";
    }
    
    /// <summary>
    /// Key of the job.
    /// </summary>
    public string Key { get; set; }
    
    /// <summary>
    /// Recurrence time for the job.
    /// </summary>
    public TimeSpan? Recurrence { get; set; }
    
    /// <summary>
    /// Next execution time of the job.
    /// </summary>
    public DateTime NextExecutionTime { get; set; }

    /// <summary>
    /// All executions of the job.
    /// </summary>
    public List<Execution> Executions { get; set; } = [];
    
    /// <summary>
    /// Type of the class in the job.
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// Method name of the class for which is job created.
    /// </summary>
    public string MethodName { get; set; }

    /// <summary>
    /// Arguments of the job.
    /// </summary>
    public List<Argument> Arguments { get; set; } = [];
    
    /// <summary>
    /// Moves execution time for the recurrent job.
    /// </summary>
    internal void MoveExecutionTime()
    {
        if (Recurrence.HasValue)
        {
            NextExecutionTime = NextExecutionTime.Add(Recurrence.Value);
        }
    }
    
    /// <summary>
    /// If the job is recurrent.
    /// </summary>
    private bool IsRecurrent() => Recurrence.HasValue;
    
    /// <inheritdoc /> 
    public JobDto? ToDto(int recursionDepth)
    {
        if (recursionDepth == 0) return null;
        
        return new JobDto(Id, Key, Type,
            MethodName, IsRecurrent(), 
            IsRecurrent() ? NextExecutionTime.ToString(CultureInfo.InvariantCulture) : "No next execution",
            Executions.Select(e => e.ToDto(recursionDepth - 1)).ToList());
    }
}