using System.Linq.Expressions;
using System.Text.Json;
using SimpleScheduler.Entities;
using SimpleScheduler.Services;

namespace SimpleScheduler;

/// <summary>
/// Class for adding jobs to the storage.
/// </summary>
public static class Jobs
{
    private static IStorage _storage = null!;
    
    /// <summary>
    /// Sets a storage to use for jobs.
    /// </summary>
    public static void SetStorage(IStorage storage) => _storage = storage;
    
    /// <summary>
    /// Executes a job once.
    /// </summary>
    public static async Task AddInstantJob(JobInfo jobInfo, JobSettings jobSettings)
    {
        var job = new Job
        {
            JobInfo = jobInfo,
            JobSettings = jobSettings
        };
        await _storage.AddJob(job);
        
        /*
        var a = JsonSerializer.Serialize(job);
        var b = JsonSerializer.Serialize(jobSettings);

        var aa = (JobInfo?)JsonSerializer.Deserialize(a, typeof(JobInfo));
        var bb = (JobSettings?)JsonSerializer.Deserialize(b, typeof(JobSettings));

        var objectData = bb!.Data!.ToString();
        var bbb = JsonSerializer.Deserialize(objectData!, Type.GetType(bb.DataType!)!);
        */
    }
}

public interface IValidatable<out T>
{
    T Validate();
}

public class JobSettings
{
    public TimeSpan Recurrence { get; set; }
    
    public TimeSpan Delay { get; set; }

    public RetrySchedule RetrySchedule { get; set; } = new();
    
    public string? Data { get; set; }

    public string? DataType { get; set; }
}


public class RetrySchedule
{
    public TimeSpan[] Retries { get; set; } = [];
}

public class ArgumentBuilder
{
    private readonly JobSettings _jobSettings = new();
    
    public JobSettings Build()
        => _jobSettings;

    public ArgumentBuilder SetRecurrence(TimeSpan recurrence)
        => LambdaReturn(() => _jobSettings.Recurrence = recurrence);

    public ArgumentBuilder SetDelay(TimeSpan delay)
        => LambdaReturn(() => _jobSettings.Delay = delay);

    public ArgumentBuilder SetRetrySchedule(params TimeSpan[] args)
        => LambdaReturn(() => _jobSettings.RetrySchedule.Retries = args);
    
    public ArgumentBuilder SetRetrySchedule(TimeSpan retryTime, int count)
        => SetRetrySchedule(Enumerable.Range(0, count).Select(_ => retryTime).ToArray());
    
    public ArgumentBuilder SetData<T>(T data)
    {
        _jobSettings.DataType = typeof(T).FullName;
        _jobSettings.Data = JsonSerializer.Serialize(data);
        return this;
    }

    private ArgumentBuilder LambdaReturn(Action action)
    {
        action();
        return this;
    }
}

public class JobInfo : IValidatable<JobInfo>
{
    public string Type { get; set; } = null!;

    public string MethodName { get; set; } = null!;
    
    public string? Key { get; set; }
    
    public JobInfo Validate()
    {
        if (Type == null)
        {
            throw new NullReferenceException($"{nameof(Type)} name is null");
        }
        if (MethodName == null)
        {
            throw new NullReferenceException($"{nameof(MethodName)} name is null");
        }

        return this;
    }
}

public class SimpleSchedulerJobContext
{
    private readonly object? _data;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public SimpleSchedulerJobContext(object? data, int retryCount)
    {
        _data = data;
        RetryCount = retryCount;
    }
    
    /// <summary>
    /// Returns data that were added for the job.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T? GetData<T>() => (T?)_data;
    
    /// <summary>
    /// How many times the job is retried.
    /// </summary>
    public int RetryCount { get; }
}

public class JobInfoBuilder<T>
{
    private readonly JobInfo _job = new();

    public JobInfoBuilder<T> SetJob(Expression<Func<T, SimpleSchedulerJobContext, Task>> selector)
        => LambdaReturn(() =>
        {
            if (selector is LambdaExpression { Body: MethodCallExpression methodCallExpression })
            {
                _job.MethodName = methodCallExpression.Method.Name;
                _job.Type = typeof(T).FullName!;
                return;
            }
            throw new NotSupportedException("Expression is not supported by simple scheduler.");
        });
        
    
    public JobInfoBuilder<T> SetKey(string key)
        => LambdaReturn(() => _job.Key = key);
    
    public JobInfo Build()
        => _job;
    
    private JobInfoBuilder<T> LambdaReturn(Action action)
    {
        action();
        return this;
    }
}
