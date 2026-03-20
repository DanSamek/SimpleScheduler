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
    public IEnumerable<Func<Task>> MapJobKeys(IEnumerable<string> jobsKeys)
    {
        foreach (var jobKey in jobsKeys)
        {
            if (_map.TryGetValue(jobKey, out var job))
            {
                yield return job;
            }
        }
    }
}