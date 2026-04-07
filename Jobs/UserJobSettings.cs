namespace SimpleScheduler.Jobs;

/// <summary>
/// Represents configuration settings for a scheduled job.
/// </summary>
public class UserJobSettings
{
    /// <summary>
    /// The recurrence interval for the job.
    /// When set, the job will execute repeatedly at the specified interval.
    /// </summary>
    internal TimeSpan? Recurrence { get; set; }
    
    /// <summary>
    /// The initial delay before the job is first executed.
    /// </summary>
    internal TimeSpan? Delay { get; set; }
    
    /// <summary>
    /// The retry schedule for the job.
    /// Each value represents the delay before a retry attempt.
    /// </summary>
    internal TimeSpan[] Retries { get; set; } = [];
    
    /// <summary>
    /// The serialized job data.
    /// </summary>
    internal string? Data { get; set; }

    /// <summary>
    /// The fully qualified type name of the serialized job data.
    /// </summary>
    internal string? DataType { get; set; }
}
