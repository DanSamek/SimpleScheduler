using SimpleScheduler.Entities;
using SimpleScheduler.Mapper;
using SimpleScheduler.Storage;
using SimpleScheduler.ThreadPool;

namespace SimpleScheduler.Scheduler;

public class Scheduler
{
    private readonly ThreadPool.ThreadPool _threadPool;
    private readonly IStorage _storage;
    private readonly IJobMapper _jobMapper;

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
                var executions = await _storage.JobsToRun();
                var executionsWithJobs = _jobMapper.GetTaskForExecutions(executions);

                foreach (var execution in executionsWithJobs)
                {
                    await Enqueue(execution);
                }

                var nearestTimeForNextJob = await _storage.NearestExecutionTimeForJob();
                if (nearestTimeForNextJob.Ticks > 0)
                {
                    await Task.Delay(nearestTimeForNextJob);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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

    public async Task OnException(int executionId, Exception? exception)
    {
        await _storage.SetExecutionFailedState(executionId, $"{exception?.Message}\n{exception?.StackTrace}");
    }
    
    /// <summary>
    /// Schedules a job on the thread pool.
    /// </summary>
    /// <param name="id">Id of the job</param>
    internal async Task<bool> ScheduleJob(int id)
    {
        var execution = await _storage.GetExecutionByJobId(id);
        if (execution == null) return false;
        
        var executionWithJob = _jobMapper.GetTaskForExecutions([execution]).First();
        await Enqueue(executionWithJob);
        return true;
    }

    private async Task Enqueue(ExecutionWithJob executionWithJob)
    {
        var data = new WorkerData(executionWithJob,this);
        await OnEnqueued(executionWithJob.Execution.Id);
        await _threadPool.EnqueueJob(data);
    }
}