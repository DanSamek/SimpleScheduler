using System.ComponentModel.DataAnnotations;

namespace SimpleScheduler.Entities;

public class Job
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

    /// <summary>
    /// Ef constructor!
    /// </summary>
    public Job()
    {
        Key = "";
    }
    
    /// <summary>
    /// Id of the job.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// State of the job.
    /// </summary>
    public JobState State { get; set; } = JobState.Inactive;
    
    /// <summary>
    /// Key of the job.
    /// </summary>
    public string Key { get; private set; }
    
    /// <summary>
    /// Recurrence time for the job.
    /// </summary>
    public TimeSpan? Recurrence { get; private set; }
    
    /// <summary>
    /// Execution time of the job.
    /// </summary>
    public DateTime ExecutionTime { get; private set; }
    
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