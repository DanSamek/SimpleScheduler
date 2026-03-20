namespace SimpleScheduler;

public class JobMapper : IJobMapper
{
    private readonly Dictionary<string, Func<Task>> _map = new();
    
    public void AddJob(Func<Task> job, string key)
    {
        Console.WriteLine($"Adding job: {key}");
        _map.Add(key, job);
    }

    public IEnumerable<Func<Task>> MapJobKeys(List<string> jobsKeys)
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