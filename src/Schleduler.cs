namespace SimpleScheduler;

public class Schleduler
{
    private readonly ThreadPool _threadPool;
    private readonly IStorage _storage;
    
    private readonly PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

    /// <summary>
    /// .Ctor
    /// </summary>
    public Schleduler(ThreadPool threadPool, IStorage storage)
    {
        _threadPool = threadPool;
        _storage = storage;
    }

    /// <summary>
    /// Runs scheduling.
    /// </summary>
    public void Run()
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        Task.Run(Loop);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }

    private async Task Loop()
    {
        while (true)
        {
            var jobs = _storage.JobsToRun();
            foreach (var job in jobs) _threadPool.EnqueueJob(job);
            
            Console.WriteLine("Waiting for jobs");
            await _timer.WaitForNextTickAsync();
        }
    }
}