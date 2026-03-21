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
        ExecutionTime = DateTime.UtcNow.Add(delay ?? TimeSpan.Zero);
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
    /// Execution time of the job.
    /// </summary>
    public DateTime ExecutionTime { get; set; }

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
            ExecutionTime = ExecutionTime.Add(Recurrence.Value);
        }
    }
    
    /// <summary>
    /// If the job is recurent.
    /// </summary>
    public bool IsRecurrent() => Recurrence.HasValue;
}