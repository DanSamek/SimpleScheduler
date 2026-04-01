using System.Threading.Channels;
using SimpleScheduler.Mapper;

namespace SimpleScheduler.ThreadPool;

internal class Worker
{
    private readonly ChannelReader<WorkerData> _channel;
    private readonly int _id;
    private readonly int _numberOfRetries;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public Worker(ChannelReader<WorkerData> channel, int id, int numberOfRetries)
    {
        _channel = channel;
        _id = id;
        _numberOfRetries = numberOfRetries;
    }
    
    public async Task Run()
    {
        while (true)
        {
            var (executionWithJob, scheduler) = await _channel.ReadAsync();
            var executionId = executionWithJob.Execution.Id;

            List<Exception> exceptions = [];
            try
            {
                await Run(scheduler, executionId, executionWithJob, 0);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            if (exceptions.Count == 0) continue;

            var ran = await RetryRun(scheduler, executionId, executionWithJob, exceptions);
            if (ran) continue;
            await scheduler.OnException(executionId, exceptions);
        }
    }

    private async Task<bool> RetryRun(Scheduler.Scheduler scheduler, int executionId,
                                      ExecutionWithJob executionWithJob, List<Exception> exceptions)
    {
        var ran = false;
        for (var @try = 1; @try <= _numberOfRetries; @try++)
        {
            try
            {
                await Run(scheduler, executionId, executionWithJob, @try);
                ran = true;
                break;
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }
        return ran;
    }
    
    private static async Task Run(Scheduler.Scheduler scheduler, int executionId,
                           ExecutionWithJob executionWithJob, int retryCount)
    {
        await scheduler.OnRunning(executionId, retryCount);
        var job = (Task?)executionWithJob.MethodInfo.Invoke(executionWithJob.Object, executionWithJob.Arguments);
        if (job == null)
        {
            throw new NullReferenceException("Job is null");
        }
        await job;
        await scheduler.OnEnded(executionId, retryCount);
    }
}