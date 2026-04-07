using SimpleScheduler.Entities;
using SimpleScheduler.Mapper;
using SimpleScheduler.Services;
using SimpleScheduler.ThreadPool;

namespace SimpleScheduler.Scheduler;

public class Scheduler
{
    private readonly ThreadPool.ThreadPool _threadPool;
    private readonly IStorage _storage;
    private readonly IJobMapper _jobMapper;
    
    private readonly CancellationTokenSource _onEnqueueCancellationToken = new CancellationTokenSource();
    
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
        // Application could be stopped when some tasks are in the queue.
        await RestoreState();
        
        while (true)
        {
            try
            {
                var retryExecutions = await _storage.ExecutionsToRetry();
                var executions = await _storage.JobsToRun();
                retryExecutions.AddRange(executions);
                
                var executionsWithJobs = _jobMapper.GetTaskForExecutions(retryExecutions);

                foreach (var execution in executionsWithJobs)
                {
                    await Enqueue(execution);
                }

                var nearestTimeForNextJob = await _storage.NearestExecutionTimeForJob();
                if (nearestTimeForNextJob.Ticks > 0)
                {
                    await Wait(nearestTimeForNextJob);
                }
            }
            catch (Exception e)
            {
                if (e is not ObjectDisposedException)
                {
                    Console.WriteLine(e);    
                }
            }
        } 
    }

    
    static readonly TimeSpan _step = new TimeSpan(0xfffffffe - 1);
    private async Task Wait(TimeSpan ts)
    {
        if (ts.Ticks < 0xfffffffe)
        {
            await Task.Delay(ts, _onEnqueueCancellationToken.Token);
        }
        while (ts.Ticks > 0)
        {
            await Task.Delay(_step, _onEnqueueCancellationToken.Token);
            ts = ts.Subtract(_step);
        }
        
        _onEnqueueCancellationToken.TryReset();   
    }
    
    private async Task RestoreState()
    {
        // TODO test it with not inmemory-storage!
        var notEndedExecutions = await _storage.NotEndedExecutions();
        notEndedExecutions = notEndedExecutions
            .OrderByDescending(e => e.State)
            .ThenBy(e => e.Created)
            .ToList();
        
        var executionsWithJobs = _jobMapper.GetTaskForExecutions(notEndedExecutions);
        foreach (var execution in executionsWithJobs)
        {
            await Enqueue(execution);
        }
    }
    
    private async Task OnEnqueued(int executionId)
    {
        await _storage.UpdateExecutionState(executionId, ExecutionState.Queued);
    }
    
    internal async Task OnRunning(int executionId)
    {
        await _storage.UpdateExecutionState(executionId, ExecutionState.Running);
    }
    
    internal async Task OnEnded(int executionId)
    {
        await _storage.UpdateExecutionState(executionId, ExecutionState.Ended);
    }

    internal async Task OnException(int executionId, Exception exception)
    {
        var canBeRetried = await _storage.CanBeRetried(executionId);
        if (canBeRetried)
        {
            await _storage.RetryExecution(executionId);
            await _onEnqueueCancellationToken.CancelAsync();
        }
        else
        {
            var errorMessage = $"{exception.Message}\n{exception.StackTrace}";
            await _storage.SetExecutionFailedState(executionId, errorMessage);   
        }
    }

    private async Task Enqueue(ExecutionWithJob executionWithJob)
    {
        var data = new WorkerData(executionWithJob,this);
        await OnEnqueued(executionWithJob.Execution.Id);
        await _threadPool.EnqueueJob(data);
        await _onEnqueueCancellationToken.CancelAsync();
    }
}