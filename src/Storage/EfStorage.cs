using Microsoft.EntityFrameworkCore;
using SimpleScheduler.Entities;

namespace SimpleScheduler.Storage;

public class EfStorage : IStorage
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public EfStorage(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
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
    public IReadOnlyList<string> JobsKeysToRun()
    {
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.GetSchedulerContext();
        
        var now = DateTime.UtcNow;
        var keys = context.Jobs
            .Where(j => j.ExecutionTime <= now && j.State == JobState.Inactive)
            .Select(j => j.Key)
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

        /*
        NOT IN-MEMORY db solution
        await context.Jobs
            .Where(j => j.Key == jobKey)
            .ExecuteUpdateAsync(spc => spc.SetProperty(j => j.State, newState));
            */
    }

    /// <inheritdoc />
    public void SetEndedState(string jobKey)
    {
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.GetSchedulerContext();
        
        var job = context.Jobs
            .FirstOrDefault(j => j.Key == jobKey);

        if (job == null) return;
        
        job.MoveExecutionTime();
        var state = job.IsRecurrent() ? JobState.Inactive : JobState.Ended;
        job.State = state;
        context.Jobs.Update(job);
        context.SaveChanges();
    }
}

public static class ServiceScopeExtensions
{
    public static SimpleSchedulerContext GetSchedulerContext(this IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<SimpleSchedulerContext>();
    }
}