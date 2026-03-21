namespace SimpleScheduler.Entities;

public class Job : DoId
{
    /// <summary>
    /// Ctor
    /// </summary>
    public Job(string key, TimeSpan? recurrence = null, TimeSpan? delay = null)
    {
        Key = key;
        Recurrence = recurrence;
        NextExecutionTime = DateTime.UtcNow.Add(delay ?? TimeSpan.Zero);
    }

    public Job()
    {
        Key = "";
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