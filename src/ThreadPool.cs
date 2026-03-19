using System.Collections.Concurrent;

namespace SimpleScheduler;

/// <summary>
/// Thread pool for executing tasks. 
/// </summary>
public class ThreadPool
{
    private readonly Worker[] _workers;
    private readonly ConcurrentQueue<Func<Task>>[] _jobQueues;
    
    public ThreadPool(int numberOfWorkers)
    {
        _jobQueues = new ConcurrentQueue<Func<Task>>[numberOfWorkers];
        
        for (var i = 0; i < _jobQueues.Length; i++) _jobQueues[i] = new ConcurrentQueue<Func<Task>>();
        
        _workers = new  Worker[numberOfWorkers];
        for (var i = 0; i < numberOfWorkers; i++) _workers[i] = new Worker(_jobQueues[i], i);
    }

    /// <summary>
    /// Runs thread pool.
    /// </summary>
    public void Run()
    {
        foreach (var worker in _workers)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() => worker.Run());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
    
    /// <summary>
    /// Enqueues a job to run on the thread pool worker.
    /// </summary>
    internal void EnqueueJob(Func<Task> job)
    {
        var minWorkQueue = _jobQueues.MinBy(x => x.Count);
        minWorkQueue!.Enqueue(job);
    }
}