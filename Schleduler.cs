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
                var executions = await _storage.JobsToRun();
                var executionsWithJobs = _jobMapper.GetTaskForExecutions(executions);

                foreach (var execution in executionsWithJobs)
                {
                    var data = new WorkerData(execution, this);
                    await OnEnqueued(execution.Execution.Id);
                    _threadPool.EnqueueJob(data);
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

    public async Task OnException(int executionId, Exception exception)
    {
        await _storage.SetExecutionFailedState(executionId, exception.StackTrace ?? string.Empty);
    }
}