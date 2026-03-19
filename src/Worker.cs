using System.Collections.Concurrent;

namespace SimpleScheduler;

internal class Worker
{
    private readonly ConcurrentQueue<Func<Task>> _jobQueue;
    private readonly int _id;
    
    public Worker(ConcurrentQueue<Func<Task>> jobQueue, int id)
    {
        _jobQueue = jobQueue;
        _id = id;
    }
    
    public async Task Run()
    {
        while (true)
        {
            if (!_jobQueue.TryDequeue(out var job)) continue;
            Console.WriteLine($"Worker {_id} is processing job.");
            await job();
        }
    }
    
}