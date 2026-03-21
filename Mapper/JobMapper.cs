using SimpleScheduler.Entities;

namespace SimpleScheduler.Mapper;

public class JobMapper : IJobMapper
{
    private readonly Dictionary<string, Func<Task>> _map = new();
    
    /// <inheritdoc />
    public void AddJob(Func<Task> job, string key)
    {
        _map.Add(key, job);
    }

    public IEnumerable<ExecutionWithJob> GetTaskForExecutions(IEnumerable<Execution> executions)
    {
        var result = executions
            .Select(e => new ExecutionWithJob(e, _map.GetValueOrDefault(e.Job.Key)));
        return result;
    }
}