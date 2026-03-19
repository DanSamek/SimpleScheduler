namespace SimpleScheduler;

public class InMemoryStorage : IStorage
{
    private readonly List<Job> _jobs = [];
    
    /// <inheritdoc /> 
    public void AddJob(Job job)
    {
        _jobs.Add(job);
    }
    
    /// <inheritdoc /> 
    public List<Func<Task>> JobsToRun()
    {
        var instantJobs = _jobs
            .Where(j => j is { CanBeExecuted: true, IsRecurrent: false })
            .ToList();

        var recurrentJobs = _jobs
            .Where(j => j is { CanBeExecuted: true, IsRecurrent: true })
            .ToArray();

        foreach (var recurrentJob in recurrentJobs)
        {
            recurrentJob.MoveExecutionTime();
        }

        foreach (var instantJob in instantJobs)
        {
            _jobs.Remove(instantJob);
        }
        
        var jobsToRun = new List<Func<Task>>();
        jobsToRun.AddRange(recurrentJobs.Select(j => j.Value));
        jobsToRun.AddRange(instantJobs.Select(j => j.Value));
        return jobsToRun;
    }
}