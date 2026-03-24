using SimpleScheduler.Entities;

namespace SimpleScheduler.Mapper;

public class JobMapper : IJobMapper
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
        var result = new List<ExecutionWithJob>();
        foreach (var execution in executions)
        {
            if (execution.Job == null)
            {
                throw new NullReferenceException("Job is null - not included.");
            }
            
            var typeName = execution.Job.Type;
            var methodName = execution.Job.MethodName;

            using var scope = _scopeFactory.CreateScope();
            var type = Type.GetType(typeName);
            
            var service = scope.ServiceProvider.GetService(type!);
            var method = service?.GetType().GetMethod(methodName);

            if (method == null)
            {
                throw new NullReferenceException($"Could not find method {methodName} in type {typeName}.");
            }
            
            var arguments = execution.Job.CreateArguments();
            var executionWithJob = new ExecutionWithJob(execution, method, service, arguments);
            result.Add(executionWithJob);
        }
        
        return result;
    }
}