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
            try
            {
                await scheduler.OnRunning(data.Key);
                await data.Job();
                await scheduler.OnEnded(data.Key);
            }
            catch (Exception ex)
            {
                await scheduler.OnException(data.Key, ex);
            }
        }   
    }
}