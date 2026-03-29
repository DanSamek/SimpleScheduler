using Microsoft.EntityFrameworkCore;
using SimpleScheduler.ContextProvider;
using SimpleScheduler.Entities;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Services;

public class EfStorage : IStorage
{
    private readonly DbContextProvider _dbContextProvider;
    private readonly SchedulerHubNotifier _hubNotifier; 
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public EfStorage(DbContextProvider dbContextProvider, SchedulerHubNotifier hubNotifier)
    {
        _dbContextProvider = dbContextProvider;
        _hubNotifier = hubNotifier;
    }
    
    /// <inheritdoc /> 
    public async Task<Job> AddJob(Job job)
    {
        return await _dbContextProvider.WithContext(async context =>
        {
            var jobs = context.Set<Job>();

            jobs.Add(job);
            await context.SaveChangesAsync();
            return job;
        });
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Execution>> JobsToRun()
    {
        return await _dbContextProvider.WithContext(async context =>
        {
            var jobs = context.Set<Job>();
        
            var now = DateTime.UtcNow;
            var jobsToRun = jobs
                .Include(j => j.Arguments)
                .ThenInclude(a => a.Arguments)
                .Where(j => j.NextExecutionTime <= now && (j.Recurrence != null || j.Executions.Count == 0))
                .ToArray();
        
            foreach (var job in jobsToRun)
            {
                job.MoveExecutionTime();
            }
        
            var executions = jobsToRun
                .Select(job => new Execution { Job = job } )
                .ToList();

            context.Set<Execution>().AddRange(executions);
            await context.SaveChangesAsync();

            foreach (var execution in executions)
            {
                await _hubNotifier.NotifyClients(execution);
            }
        
            return executions;
        });
    }

    public async Task UpdateExecutionState(int executionId, ExecutionState newState)
    {
        await _dbContextProvider.WithContext(async context =>
        {
            var executions = context.Set<Execution>();

            var execution = executions
                .Include(e => e.Job)
                .FirstOrDefault(e => e.Id == executionId);
        
            if (execution is null) return;

            if (newState == ExecutionState.Running)
            {
                execution.Started = DateTime.UtcNow;
            }
            else if (newState == ExecutionState.Ended)
            {
                execution.Ended = DateTime.UtcNow;
            }
        
            execution.State = newState;
            context.Update(execution);
            await context.SaveChangesAsync();
        
            await _hubNotifier.NotifyClients(execution);
        });
    }
    
    /// <inheritdoc />
    public async Task SetExecutionFailedState(int executionId, string errorMessage)
    {
        await _dbContextProvider.WithContext(async context =>
        {
            var executions = context.Set<Execution>();
            var execution = executions
                .Include(e => e.Job)
                .FirstOrDefault(e => e.Id == executionId);

            if (execution is null) return;

            execution.State = ExecutionState.Failed;
            execution.Error = errorMessage;
            execution.Ended = DateTime.UtcNow;

            context.Update(execution);
            await context.SaveChangesAsync();
        
            await _hubNotifier.NotifyClients(execution);
        });
        
    }
    
    /// <inheritdoc />
    public async Task<List<Execution>> ExecutionsPage(int pageIndex)
    {
        return await _dbContextProvider.WithContext(context =>
        {
            var executions = context.Set<Execution>()
                .Include(e => e.Job)
                .OrderByDescending(e => e.Id)
                .Skip(pageIndex * Constants.PAGE_SIZE)
                .Take(Constants.PAGE_SIZE)
                .ToList();
            
            return Task.FromResult(executions);
        });
    }

    /// <inheritdoc />
    public async Task<TimeSpan> NearestExecutionTimeForJob()
    {
        return await _dbContextProvider.WithContext(context =>
        {
            var nearestExecutionTime = context.Set<Job>()
                .OrderBy(j => j.NextExecutionTime)
                .Select(j => j.NextExecutionTime)
                .FirstOrDefault();

            var now = DateTime.UtcNow;

            var result = nearestExecutionTime - now;
            return Task.FromResult(result);
        });
    }
    
    /// <inheritdoc />
    public async Task<List<Job>> AllJobs()
    {
        return await _dbContextProvider.WithContext(context =>
        {
            var jobs = context.Set<Job>().ToList();
            return Task.FromResult(jobs);
        });
    }
    
    /// <inheritdoc />
    public async Task<Execution?> GetExecutionByJobId(int jobId)
    {
        return await _dbContextProvider.WithContext(async context =>
        {
            var job = context.Set<Job>()
                .Include(j => j.Arguments)
                .ThenInclude(a => a.Arguments)
                .FirstOrDefault(j => j.Id == jobId);
            
            if (job is null) return null;

            var execution = new Execution { Job = job };
            context.Set<Execution>().Add(execution);
            await context.SaveChangesAsync();
            return execution;
        });
    }

    
    /// <inheritdoc />
    public async Task<Execution?> ExecutionById(int id)
    {
        return await _dbContextProvider.WithContext(context =>
        {
            var execution = context.Set<Execution>()
                .AsNoTracking()
                .Include(e => e.Job)
                .ThenInclude(j => j!.Arguments)
                .ThenInclude(a => a.Arguments)
                .FirstOrDefault(j => j.Id == id);

            return Task.FromResult(execution);
        });
    }
    
    /// <inheritdoc />
    public async Task<int> TotalExecutionPages()
    {
        return await _dbContextProvider.WithContext(context => Task.FromResult((int)double.Ceiling(context.Set<Execution>().Count() * 1.0 / Constants.PAGE_SIZE)));
    }
    
    /// <inheritdoc />
    public async Task<int> TotalExecutions()
    {
        return await _dbContextProvider.WithContext(context => Task.FromResult(context.Set<Execution>().Count()));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Execution>> NotEndedExecutions()
    {
        return await _dbContextProvider.WithContext(context =>
        {
            var result = context.Set<Execution>().Where(e =>
                e.State == ExecutionState.Created ||
                e.State == ExecutionState.Queued ||
                e.State == ExecutionState.Running)
                .ToArray();
            
            return Task.FromResult(result);
        });
    }
    
    /// <inheritdoc />
    public async Task<Job?> JobById(int id)
    {
        return await _dbContextProvider.WithContext(context =>
        {
            var job = context.Set<Job>()
                .AsNoTracking()
                .Include(j => j.Arguments)
                .ThenInclude(a => a.Arguments)
                .FirstOrDefault(e => e.Id == id);
            
            var executions = context.Set<Execution>()
                .AsNoTracking()
                .Where(e => e.Job != null && e.Job.Id == id)
                .OrderByDescending(e => e.Created)
                .Take(Constants.PAGE_SIZE)
                .ToList();
            
            job?.Executions = executions;
            return Task.FromResult(job);
        });
    }
}
