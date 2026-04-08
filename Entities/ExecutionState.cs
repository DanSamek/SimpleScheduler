namespace SimpleScheduler.Entities;

/// <summary>
/// State of the execution of the job.
/// </summary>
internal enum ExecutionState
{
    Created = 0,
    WaitingForRetry = 1,
    Queued = 2,
    Running = 3,
    Ended = 4,
    Failed = 5,
}