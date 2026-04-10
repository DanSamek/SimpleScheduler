using SimpleScheduler.Entities.Db;

namespace SimpleScheduler.Mapper;

/// <summary>
/// Interface for the job mapper.
/// It's a in-memory cache for jobs.
/// </summary>
internal interface IJobMapper
{
    /// <summary>
    /// Returns mapped jobs to the specified keys.  
    /// </summary>
    IEnumerable<ExecutionWithJob> GetTaskForExecutions(IEnumerable<Execution> executions);
}
