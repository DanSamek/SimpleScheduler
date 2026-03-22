namespace SimpleScheduler.Entities;

/// <summary>
/// State of the execution of the job.
/// </summary>
public enum ExecutionState
{
    Created = 0,
    Queued = 1,
    Running = 2,
    Ended = 3,
    Failed = 4,
}