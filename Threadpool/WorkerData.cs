namespace SimpleScheduler.ThreadPool;

/// <summary>
/// Data for worker's job queue. 
/// </summary>
internal record WorkerData(Func<Task> Job, string Key, Scheduler Scheduler);