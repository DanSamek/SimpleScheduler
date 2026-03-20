namespace SimpleScheduler;

public class Job
{
    private DateTime _executionTime;
    private readonly TimeSpan? _recurrence;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public Job(string jobKey, TimeSpan? delay = null, TimeSpan? recurrence = null)
    {
        JobKey = jobKey;
        _executionTime = DateTime.UtcNow.Add(delay ?? TimeSpan.Zero);
        _recurrence = recurrence;
    }
    
    public String JobKey { get; }

    public bool IsRecurrent => _recurrence.HasValue;
    public bool CanBeExecuted => _executionTime <=  DateTime.UtcNow;
    public void MoveExecutionTime()
    {
        ArgumentNullException.ThrowIfNull(_recurrence);
        _executionTime = _executionTime.Add(_recurrence.Value);
    }
}