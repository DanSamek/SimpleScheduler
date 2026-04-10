using SimpleScheduler.Entities;
using SimpleScheduler.Mapper;
using SimpleScheduler.Services;
using SimpleScheduler.ThreadPool;

namespace SimpleScheduler.Scheduler;

internal  class Scheduler
{
    private readonly ThreadPool.ThreadPool _threadPool;
    private readonly IStorage _storage;
    private readonly IJobMapper _jobMapper;
    private readonly ErrorLogger _errorLogger;
    
    private CancellationTokenSource _onEnqueueCancellationToken = new CancellationTokenSource();
    private readonly SemaphoreSlim _cancellationTokenSemaphore = new SemaphoreSlim(1,1);
    private readonly SemaphoreSlim _executionSemaphore = new SemaphoreSlim(1,1);
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public Scheduler(ThreadPool.ThreadPool threadPool, IStorage storage, IJobMapper jobMapper, ErrorLogger errorLogger)
    {
        _threadPool = threadPool;
        _storage = storage;
        _jobMapper = jobMapper;
        _errorLogger = errorLogger;
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

                await _executionSemaphore.WaitAsync();
                TimeSpan nearestTimeForNextJob;
                try
                {
                    nearestTimeForNextJob = await _storage.NearestExecutionTimeForJob();
                }
                finally
                {
                    _executionSemaphore.Release();
                }
                if (nearestTimeForNextJob.Ticks > 0)
                {
                    await Wait(nearestTimeForNextJob);
                }
               
            }
            catch (Exception e)
            {
                #if DEBUG
                if (e is TaskCanceledException)
                {
                    Console.WriteLine("Waiting was canceled.");
                }
                else 
                #endif 
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
        await _cancellationTokenSemaphore.WaitAsync();
        try
        {
            if (_onEnqueueCancellationToken.IsCancellationRequested)
            {
                _onEnqueueCancellationToken = new CancellationTokenSource();
                return;
            }
        }
        finally
        {
            _cancellationTokenSemaphore.Release();
        }
        
        if (ts.Ticks < 0xfffffffe)
        {
            await Task.Delay(ts, _onEnqueueCancellationToken.Token);
            return;
        }
        while (ts.Ticks > 0)
        {
            await Task.Delay(_step, _onEnqueueCancellationToken.Token);
            ts = ts.Subtract(_step);
        }   
        
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
        await _errorLogger.AddError(exception, executionId);
        
        if (canBeRetried)
        {
            await _executionSemaphore.WaitAsync();
            try
            {
                await _storage.RetryExecution(executionId);
            }
            finally
            {
                _executionSemaphore.Release();
            }

            await _cancellationTokenSemaphore.WaitAsync();
            try
            {
                await _onEnqueueCancellationToken.CancelAsync();
            }
            finally
            {
                _cancellationTokenSemaphore.Release();
            }
        }
        else
        {
            await _storage.SetExecutionFailedState(executionId);
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