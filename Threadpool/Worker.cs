using System.Collections.Concurrent;

namespace SimpleScheduler.ThreadPool;

internal class Worker
{
    private readonly ConcurrentQueue<WorkerData> _jobQueue;
    private readonly int _id;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public Worker(ConcurrentQueue<WorkerData> jobQueue, int id)
    {
        _jobQueue = jobQueue;
        _id = id;
    }
    
    public async Task Run()
    {
        while (true)
        {
            if (!_jobQueue.TryDequeue(out var data)) continue;
            var scheduler = data.Scheduler;
            var executionWithJob = data.ExecutionWithJob;
            var executionId = executionWithJob.Execution.Id;
            try
            {
                await scheduler.OnRunning(executionId);
                var job = (Task?)executionWithJob.MethodInfo.Invoke(executionWithJob.Object, null);
                if (job == null)
                {
                    throw new NullReferenceException("Job is null");
                }

                await job;
                await scheduler.OnEnded(executionId);
            }
            catch (Exception ex)
            {
                await scheduler.OnException(executionId, ex);
            }
        }   
    }
}