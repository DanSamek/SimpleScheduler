using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SimpleScheduler.ContextProvider;
using SimpleScheduler.Entities;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Services;

internal class EfStorage : IStorage
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
                .Include()
                .Where(j => j.NextExecutionTime <= now)
                .ToArray();
        
            foreach (var job in jobsToRun)
            {
                job.MoveExecutionTime();
            }
        
            var executions = jobsToRun
                .Select(job => new Execution { Job = job, MaxRetryCount = job.JobSettings.Retries.Length } )
                .ToList();

            context.Set<Execution>()
                .AddRange(executions);
            
            await context.SaveChangesAsync();

            foreach (var execution in executions)
            {
                await _hubNotifier.NotifyClients(execution);
            }
        
            return executions;
        });
    }

    /// <inheritdoc />
    public async Task<List<Execution>> ExecutionsToRetry()
    {
        return await _dbContextProvider.WithContext(context =>
        {
            var now = DateTime.UtcNow;
            var executions = context.Set<Execution>()
                .Where(e => e.State == ExecutionState.WaitingForRetry && e.RetryTime < now)
                .ToList();

            var requestedJobIds = executions
                .Select(e => e.JobId);

            var jobs = context.Set<Job>()
                .Include()
                .Where(j => requestedJobIds.Contains(j.Id))
                .ToDictionary(j => j.Id, j => j);

            foreach (var execution in executions)
            {
                execution.Job = jobs[execution.JobId];
            }
            return Task.FromResult(executions);
        });
    }
    
    /// <inheritdoc />
    public async Task UpdateExecutionState(int executionId, ExecutionState newState)
    {
        await _dbContextProvider.WithContext(async context =>
        {
            var executions = context.Set<Execution>();

            var execution = executions
                .Include(e => e.Job)
                .FirstOrDefault(e => e.Id == executionId);

            if (execution is null) return;
            
            var jobSettings = context.Set<JobSettings>()
                .FirstOrDefault(s => execution.Job != null && s.JobId == execution.Job.Id);

            var jobInfo = context.Set<JobInfo>()
                .FirstOrDefault(s => execution.Job != null && s.JobId == execution.Job.Id);
            
            if (newState == ExecutionState.Running) execution.Started = DateTime.UtcNow;
            else if (newState == ExecutionState.Ended) execution.Ended = DateTime.UtcNow;
            execution.State = newState;
            execution.Job!.JobInfo = jobInfo!;
            execution.Job!.JobSettings = jobSettings!;
            
            context.Update(execution);
            await context.SaveChangesAsync();
        
            await _hubNotifier.NotifyClients(execution);
        });
    }
    
    /// <inheritdoc />
    public async Task SetExecutionFailedState(int executionId)
    {
        await _dbContextProvider.WithContext(async context =>
        {
            var executions = context.Set<Execution>();
            var execution = executions
                .Include(e => e.Job)
                .FirstOrDefault(e => e.Id == executionId);

            if (execution is null) return;

            execution.State = ExecutionState.Failed;
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
            var nearestJobExecutionTime = context.Set<Job>()
                .OrderBy(j => j.NextExecutionTime)
                .Select(j => j.NextExecutionTime)
                .FirstOrDefault();
            
            var nearestRetryJobExecutionTime = context.Set<Execution>()
                .Where(e => e.State == ExecutionState.WaitingForRetry)
                .OrderBy(e => e.RetryTime)
                .Select(j => j.RetryTime)
                .FirstOrDefault();

            if (nearestRetryJobExecutionTime == default)
                nearestRetryJobExecutionTime = DateTime.MaxValue;
            if (nearestJobExecutionTime == default)
                nearestJobExecutionTime = DateTime.MaxValue;
            
            var now = DateTime.UtcNow;
            var smallerTime = nearestJobExecutionTime < nearestRetryJobExecutionTime
                ? nearestJobExecutionTime
                : nearestRetryJobExecutionTime;
            
            var result = smallerTime - now;
            return Task.FromResult(result);
        });
    }
    
    /// <inheritdoc />
    public async Task<List<Job>> AllJobs()
    {
        return await _dbContextProvider.WithContext(context =>
        {
            var jobs = context.Set<Job>()
                .Include()
                .ToList();
            return Task.FromResult(jobs);
        });
    }
    
    /// <inheritdoc />
    public async Task<Execution?> GetExecutionByJobId(int jobId)
    {
        return await _dbContextProvider.WithContext(async context =>
        {
            var job = context.Set<Job>()
                .Include(j => j.JobInfo)
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
                .ThenInclude(j => j!.JobInfo)
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
                .Include()
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

    
    /// <inheritdoc />
    public async Task<List<Execution>> GetExecutionsByState(ExecutionState state)
    {
        return await _dbContextProvider.WithContext(context =>
        {
            var executions = context.Set<Execution>()
                .AsNoTracking()
                .Include(e => e.Job)
                .Where(e => e.State == state)
                .OrderByDescending(e => e.Created)
                .Take(Constants.PAGE_SIZE)
                .ToList();
            
            return Task.FromResult(executions);
        });
    }
    
    /// <inheritdoc />
    public async Task<Job?> GetJobById(int id)
    {
        return await _dbContextProvider.WithContext(context =>
        {
            var job = context.Set<Job>()
                .AsNoTracking()
                .Include()
                .FirstOrDefault(j => j.Id == id);
            
            return Task.FromResult(job);
        });
    }

    /// <inheritdoc />
    public async Task<bool> CanBeRetried(int executionId)
    {
        return await _dbContextProvider.WithContext(context =>
        {
            var execution = context.Set<Execution>()
                .AsNoTracking()
                .Include(e => e.Job)
                .FirstOrDefault(e => e.Id == executionId);
            
            return Task.FromResult(execution != null && execution.RetryCount != execution.MaxRetryCount);
        });
    }

    /// <inheritdoc />
    public async Task RetryExecution(int executionId)
    {
        await _dbContextProvider.WithContext(context =>
        {
            var execution = context.Set<Execution>()
                .Include(e => e.Job)
                .ThenInclude(j => j!.JobSettings)
                .FirstOrDefault(e => e.Id == executionId);

            if (execution == null) return Task.CompletedTask;
            
            var increment = execution.Job!.JobSettings.Retries[execution.RetryCount];
            execution.RetryTime = execution.RetryTime.Add(increment);
            execution.State = ExecutionState.WaitingForRetry;
            execution.RetryCount++;

            context.SaveChanges();
            return Task.CompletedTask;
        });
    }
}

internal static class EfExtensions
{
    internal static IIncludableQueryable<Job, JobSettings> Include(this IQueryable<Job> queryable)
    {
        return queryable
            .Include(j => j.JobInfo)
            .Include(j => j.JobSettings);
    }
}
