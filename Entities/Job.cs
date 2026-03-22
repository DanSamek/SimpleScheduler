namespace SimpleScheduler.Entities;

public class Job : DoId
{
    /// <summary>
    /// Ctor
    /// </summary>
    public Job(string type, string methodName, string? key = null, TimeSpan? recurrence = null, TimeSpan? delay = null)
    {
        Key = key ?? methodName;
        Recurrence = recurrence;
        NextExecutionTime = DateTime.UtcNow.Add(delay ?? TimeSpan.Zero);
        Type = type;
        MethodName = methodName;
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
    /// TODO somehow handle UI stuff -- set NextExecutionTime time to "NEVER" value?
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
    /// Moves execution time for the recurrent job.
    /// </summary>
    public void MoveExecutionTime()
    {
        if (Recurrence.HasValue)
        {
            NextExecutionTime = NextExecutionTime.Add(Recurrence.Value);
        }
    }
    
    /// <summary>
    /// If the job is recurent.
    /// </summary>
    public bool IsRecurrent() => Recurrence.HasValue;
}