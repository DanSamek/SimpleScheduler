using SimpleScheduler.Entities;
using SimpleScheduler.Jobs;
using SimpleScheduler.Scheduler;

namespace SimpleScheduler.Mapper;

internal class JobMapper : IJobMapper
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public JobMapper(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    
    public IEnumerable<ExecutionWithJob> GetTaskForExecutions(IEnumerable<Execution> executions)
    {
        using var scope = _scopeFactory.CreateScope();
        
        var result = new List<ExecutionWithJob>();
        foreach (var execution in executions)
        {
            // This should not happen - our responsibility
            if (execution.Job == null) throw new NullReferenceException("Job is null - not included.");

            var info = execution.Job.JobInfo;
            var typeName = info.Type;
            var methodName = info.MethodName;

            var type = Type.GetType(typeName);
            
            var service = scope.ServiceProvider.GetService(type!);
            var method = service?.GetType().GetMethod(methodName);

            // This should not happen
            if (method == null) throw new NullReferenceException($"Could not find method {methodName} in type {typeName}.");

            var data = execution.Job.JobSettings.DeserializeData();
            var context = new SimpleSchedulerJobContext(data,execution.RetryCount); 
            
            var executionWithJob = new ExecutionWithJob(execution, method, service, [context]);
            result.Add(executionWithJob);
        }
        
        return result;
    }
}