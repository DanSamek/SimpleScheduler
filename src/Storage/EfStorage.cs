using SimpleScheduler.Entities;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Storage;

public class EfStorage : IStorage
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
        using var context = scope.GetSchedulerContext();

        var sameJob = context.Jobs.Any(j => j.Key == job.Key);
        if (sameJob) return;
        
        context.Jobs.Add(job);
        context.SaveChanges();
    }

    /// <inheritdoc />
    public IReadOnlyList<Job> JobsToRun()
    {
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.GetSchedulerContext();
        
        var now = DateTime.UtcNow;
        var keys = context.Jobs
            .Where(j => j.ExecutionTime <= now && j.State == JobState.Inactive)
            .ToArray();
        
        return keys;
    }

    /// <inheritdoc />
    public async Task UpdateJobState(string jobKey, JobState newState)
    {
        using var scope = _scopeFactory.CreateScope();
        await using var context = scope.GetSchedulerContext();

        var job = context.Jobs.FirstOrDefault(j => j.Key == jobKey);
        if (job is null) return;

        job.State = newState;
        context.Update(job);
        await context.SaveChangesAsync();
        
        await _hubNotifier.NotifyClients(job);
    }

    /// <inheritdoc />
    public async Task SetEndedState(string jobKey)
    {
        using var scope = _scopeFactory.CreateScope();
        await using var context = scope.GetSchedulerContext();
        
        var job = context.Jobs
            .FirstOrDefault(j => j.Key == jobKey);

        if (job == null) return;
        
        job.MoveExecutionTime();
        var state = job.IsRecurrent() ? JobState.Inactive : JobState.Ended;
        job.State = state;
        
        context.Jobs.Update(job);
        await context.SaveChangesAsync();
        
        await _hubNotifier.NotifyClients(job);
    }
}

public static class ServiceScopeExtensions
{
    public static SimpleSchedulerContext GetSchedulerContext(this IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<SimpleSchedulerContext>();
    }
}