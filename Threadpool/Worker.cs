using System.Reflection;
using System.Threading.Channels;

namespace SimpleScheduler.ThreadPool;

internal class Worker
{
    private readonly ChannelReader<WorkerData> _channel;
    private readonly int _id;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public Worker(ChannelReader<WorkerData> channel, int id)
    {
        _channel = channel;
        _id = id;
    }
    
    public async Task Run()
    {
        while (true)
        {
            var (executionWithJob, scheduler) = await _channel.ReadAsync();
            var executionId = executionWithJob.Execution.Id;
            try
            {
                await scheduler.OnRunning(executionId);
                var job = (Task?)executionWithJob.MethodInfo.Invoke(executionWithJob.Object, null);
                if (job == null)
                {
                    throw new NullReferenceException("Job is null");
                }

                await job;
                await scheduler.OnEnded(executionId);
            }
            catch (TargetInvocationException ex)
            {
                await scheduler.OnException(executionId, ex.InnerException);
            }
        }
    }
}