using SimpleScheduler.Entities;

namespace SimpleScheduler.Mapper;

/// <summary>
/// Interface for the job mapper.
/// It's a in-memory cache for jobs.
/// </summary>
public interface IJobMapper
{
    /// <summary>
    /// Returns mapped jobs to the specified keys.  
    /// </summary>
    IEnumerable<ExecutionWithJob> GetTaskForExecutions(IEnumerable<Execution> executions);
}
