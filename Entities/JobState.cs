namespace SimpleScheduler.Entities;

/// <summary>
/// State of the job.
/// </summary>
public enum JobState
{
    Queued = 0,
    Running = 1,
    Ended = 2,
    Inactive = 3,
    Failed = 4,
}