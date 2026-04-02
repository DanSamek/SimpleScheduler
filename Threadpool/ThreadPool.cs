using System.Threading.Channels;

namespace SimpleScheduler.ThreadPool;

/// <summary>
/// Thread pool for executing tasks. 
/// </summary>
public class ThreadPool
{
    private readonly Worker[] _workers;
    private readonly Channel<WorkerData>[] _jobChannels;
    public ThreadPool(int numberOfWorkers)
    {
        _jobChannels = new Channel<WorkerData>[numberOfWorkers];

        for (var i = 0; i < _jobChannels.Length; i++)
        {
            _jobChannels[i] = Channel.CreateUnbounded<WorkerData>();
        }
        
        _workers = new Worker[numberOfWorkers];
        for (var i = 0; i < numberOfWorkers; i++)
        {
            _workers[i] = new Worker(_jobChannels[i].Reader, i);
        }
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
    internal async Task EnqueueJob(WorkerData data)
    {
        // TODO somehow by estimated time?
        var minChannel = _jobChannels.MinBy(ch => ch.Reader.Count);
        await minChannel!.Writer.WriteAsync(data);
    }
}