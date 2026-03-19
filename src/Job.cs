namespace SimpleScheduler;

public class Job
{
    private DateTime _executionTime;
    private readonly TimeSpan? _recurrence;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public Job(Func<Task> job, TimeSpan? delay = null, TimeSpan? recurrence = null)
    {
        Value = job;
        _executionTime = DateTime.UtcNow.Add(delay ?? TimeSpan.Zero);
        _recurrence = recurrence;
    }
    
    public Func<Task> Value { get; }

    public bool IsRecurrent => _recurrence.HasValue;
    public bool CanBeExecuted => _executionTime <=  DateTime.UtcNow;
    public void MoveExecutionTime()
    {
        ArgumentNullException.ThrowIfNull(_recurrence);
        _executionTime = _executionTime.Add(_recurrence.Value);
    }
}