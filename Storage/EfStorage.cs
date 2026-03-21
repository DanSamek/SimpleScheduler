using Microsoft.EntityFrameworkCore;
using SimpleScheduler.Entities;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Storage;

public class EfStorage<TDbContext> : IStorage
    where TDbContext : DbContext
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SchedulerHubNotifier _hubNotifier; 
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public EfStorage(IServiceScopeFactory scopeFactory, SchedulerHubNotifier hubNotifier)
    {
        _scopeFactory = scopeFactory;
        _hubNotifier = hubNotifier;
    }
    
    /// <inheritdoc /> 
    public void AddJob(Job job) 
    {
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.GetSchedulerContext<TDbContext>();
        var jobs = context.Set<Job>();

        var sameJob = jobs.Any(j => j.Key == job.Key);
        if (sameJob) return;
        
        jobs.Add(job);
        context.SaveChanges();
    }

    /// <inheritdoc />
    public IReadOnlyList<Job> JobsToRun()
    {
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.GetSchedulerContext<TDbContext>();
        var jobs = context.Set<Job>();
        
        var now = DateTime.UtcNow;
        var jobsToRun = jobs
            .AsNoTracking()
            .Where(j => j.ExecutionTime <= now && j.State == JobState.Inactive)
            .ToArray();
        
        return jobsToRun;
    }

    /// <inheritdoc />
    public async Task UpdateJobState(string jobKey, JobState newState)
    {
        using var scope = _scopeFactory.CreateScope();
        await using var context = scope.GetSchedulerContext<TDbContext>();
        var jobs = context.Set<Job>();

        var job = jobs.FirstOrDefault(j => j.Key == jobKey);
        if (job is null) return;

        job.State = newState;
        context.Update(job);
        await context.SaveChangesAsync();
        
        await _hubNotifier.NotifyClients(job);
    }

    public async Task SetFailedState(string jobKey, string errorMessage)
    {
        using var scope = _scopeFactory.CreateScope();
        await using var context = scope.GetSchedulerContext<TDbContext>();
        var jobs = context.Set<Job>();

        var job = jobs.FirstOrDefault(j => j.Key == jobKey);
        if (job is null) return;

        job.State = JobState.Failed;
        job.Error = errorMessage;

        context.Update(job);
        await context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task SetEndedState(string jobKey)
    {
        using var scope = _scopeFactory.CreateScope();
        await using var context = scope.GetSchedulerContext<TDbContext>();
        var jobs = context.Set<Job>();
        
        var job = jobs .FirstOrDefault(j => j.Key == jobKey);

        if (job == null) return;
        
        job.MoveExecutionTime();
        var state = job.IsRecurrent() ? JobState.Inactive : JobState.Ended;
        job.State = state;
        
        jobs.Update(job);
        await context.SaveChangesAsync();
        
        await _hubNotifier.NotifyClients(job);
    }
    
    /// <inheritdoc />
    public List<Job> AllJobs()
    {
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.GetSchedulerContext<TDbContext>();
        var jobs = context.Set<Job>().ToList();
        return jobs;
    }
}

public static class ServiceScopeExtensions
{
    public static TDbContext GetSchedulerContext<TDbContext>(this IServiceScope scope)
        where TDbContext : DbContext
    {
        return scope.ServiceProvider.GetRequiredService<TDbContext>();
    }
}
