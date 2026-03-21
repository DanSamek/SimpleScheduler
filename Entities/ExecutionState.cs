namespace SimpleScheduler.Entities;

/// <summary>
/// State of the execution of the job.
/// </summary>
public enum ExecutionState
{
    Queued = 0,
    Running = 1,
    Ended = 2,
    Failed = 3,
}