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

    /// <inheritdoc />
    public IEnumerable<Func<Task>> GetTaskForJobs(IEnumerable<Job> jobs)
    {
        foreach (var job in jobs)
        {
            if (_map.TryGetValue(job.Key, out var fnc))
            {
                yield return fnc;
            }
        }
    }
}