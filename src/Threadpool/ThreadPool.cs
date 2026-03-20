using System.Collections.Concurrent;

namespace SimpleScheduler.ThreadPool;

/// <summary>
/// Thread pool for executing tasks. 
/// </summary>
public class ThreadPool
{
    private readonly Worker[] _workers;
    private readonly ConcurrentQueue<WorkerData>[] _jobQueues;
    
    public ThreadPool(int numberOfWorkers)
    {
        _jobQueues = new ConcurrentQueue<WorkerData>[numberOfWorkers];
        
        for (var i = 0; i < _jobQueues.Length; i++) _jobQueues[i] = new ConcurrentQueue<WorkerData>();
        
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
    internal void EnqueueJob(WorkerData data)
    {
        var minWorkQueue = _jobQueues.MinBy(x => x.Count);
        minWorkQueue!.Enqueue(data);
    }
}