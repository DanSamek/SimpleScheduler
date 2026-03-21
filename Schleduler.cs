using SimpleScheduler.Entities;
using SimpleScheduler.Mapper;
using SimpleScheduler.Storage;
using SimpleScheduler.ThreadPool;

namespace SimpleScheduler;

public class Scheduler
{
    private readonly ThreadPool.ThreadPool _threadPool;
    private readonly IStorage _storage;
    private readonly IJobMapper _jobMapper;
    
    private readonly PeriodicTimer _timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

    /// <summary>
    /// .Ctor
    /// </summary>
    public Scheduler(ThreadPool.ThreadPool threadPool, IStorage storage, IJobMapper jobMapper)
    {
        _threadPool = threadPool;
        _storage = storage;
        _jobMapper = jobMapper;
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
            try
            {
                var jobs = _storage.JobsToRun();
                var tasks = _jobMapper.GetTaskForJobs(jobs);
                
                var i = 0;
                foreach (var task in tasks)
                {
                    var job = jobs[i];
                    await _storage.UpdateJobState(job.Key, JobState.Queued);
                    var data = new WorkerData(task, job.Key, this);
                    _threadPool.EnqueueJob(data);
                    i++;
                }
                
                await _timer.WaitForNextTickAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    internal async Task OnRunning(string jobKey)
    {
        await _storage.UpdateJobState(jobKey, JobState.Running);
    }
    
    internal async Task OnEnded(string jobKey)
    {
        await _storage.SetEndedState(jobKey);
    }

    public async Task OnException(string jobKey, Exception exception)
    {
        await _storage.SetFailedState(jobKey, exception.StackTrace ?? string.Empty);
    }
}