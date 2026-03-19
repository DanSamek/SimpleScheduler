namespace SimpleScheduler;

public class InMemoryStorage : IStorage
{
    private readonly List<Job> _jobs = [];
    
    public void AddJob(Job job)
    {
        _jobs.Add(job);
    }
    
    public List<Func<Task>> JobsToRun()
    {
        var instantJobs = _jobs
            // todo delayed jobs.
            .Where(j => j.Delay is null && j.Recurrence is null);
        
        // todo recurrence jobs
        
        var result = instantJobs
            .Select(j => j.Work)
            .ToList();
        return result;
    }
}