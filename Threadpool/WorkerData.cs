using SimpleScheduler.Mapper;

namespace SimpleScheduler.ThreadPool;

/// <summary>
/// Data for worker's job queue. 
/// </summary>
internal record WorkerData(ExecutionWithJob ExecutionWithJob, Scheduler Scheduler);