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
    public IReadOnlyList<Execution> JobsToRun()
    {
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.GetSchedulerContext<TDbContext>();
        var jobs = context.Set<Job>();
        
        var now = DateTime.UtcNow;
        var jobsToRun = jobs
            .Where(j => j.ExecutionTime <= now && (j.Recurrence != null || j.Executions.Count == 0))
            .ToArray();
        
        foreach (var job in jobsToRun)
        {
            job.MoveExecutionTime();
        }
        
        var result = jobsToRun
            .Select(job => new Execution { Job = job })
            .ToList();

        context.Set<Execution>().AddRange(result);
        context.SaveChanges();
        return result;
    }

    public async Task UpdateExecutionState(int executionId, ExecutionState newState)
    {
        using var scope = _scopeFactory.CreateScope();
        await using var context = scope.GetSchedulerContext<TDbContext>();
        var executions = context.Set<Execution>();

        var execution = executions
            .Include(e => e.Job)
            .FirstOrDefault(e => e.Id == executionId);
        
        if (execution is null) return;

        execution.State = newState;
        context.Update(execution);
        await context.SaveChangesAsync();
        
        await _hubNotifier.NotifyClients(execution);
    }

    public async Task SetExecutionFailedState(int executionId, string errorMessage)
    { 
        using var scope = _scopeFactory.CreateScope();
        await using var context = scope.GetSchedulerContext<TDbContext>();
        var executions = context.Set<Execution>();

        var execution = executions
            .Include(e => e.Job)
            .FirstOrDefault(e => e.Id == executionId);
        
        if (execution is null) return;

        execution.State = ExecutionState.Failed;
        execution.Error = errorMessage;

        context.Update(execution);
        await context.SaveChangesAsync();
    }

    public List<Execution> AllExecutions()
    { 
        using var scope = _scopeFactory.CreateScope();
        using var context = scope.GetSchedulerContext<TDbContext>();
        var executions = context.Set<Execution>()
            .Include(e => e.Job)
            .ToList();
        
        return executions;
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
