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
            await data.Scheduler.OnRunning(data.Key);
            await data.Job();
            data.Scheduler.OnEnded(data.Key);
        }
    }
    
}