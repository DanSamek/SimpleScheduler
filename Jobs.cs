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
    public static async Task AddInstantJob(UserJobInfo userJobInfo, UserJobSettings userJobSettings)
    {
        var job = new Job
        {
            JobInfo = new JobInfo(userJobInfo),
            JobSettings = new JobSettings(userJobSettings)
        };
        await _storage.AddJob(job);
    }
}

public interface IValidatable<out T>
{
    T Validate();
}

public class UserJobSettings
{
    public TimeSpan? Recurrence { get; set; }
    
    public TimeSpan? Delay { get; set; }

    
    public TimeSpan[] Retries { get; set; } = [];
    
    public string? Data { get; set; }

    public string? DataType { get; set; }
}


public class ArgumentBuilder
{
    private readonly UserJobSettings _userJobSettings = new();
    
    public UserJobSettings Build()
        => _userJobSettings;

    public ArgumentBuilder SetRecurrence(TimeSpan recurrence)
        => LambdaReturn(() => _userJobSettings.Recurrence = recurrence);

    public ArgumentBuilder SetDelay(TimeSpan delay)
        => LambdaReturn(() => _userJobSettings.Delay = delay);

    public ArgumentBuilder SetRetrySchedule(params TimeSpan[] args)
        => LambdaReturn(() => _userJobSettings.Retries = args);
    
    public ArgumentBuilder SetRetrySchedule(TimeSpan retryTime, int count)
        => SetRetrySchedule(Enumerable.Range(0, count).Select(_ => retryTime).ToArray());
    
    public ArgumentBuilder SetData<T>(T data)
    {
        _userJobSettings.DataType = typeof(T).FullName;
        _userJobSettings.Data = JsonSerializer.Serialize(data);
        return this;
    }

    private ArgumentBuilder LambdaReturn(Action action)
    {
        action();
        return this;
    }
}

public class UserJobInfo : IValidatable<UserJobInfo>
{
    public string Type { get; set; } = null!;

    public string MethodName { get; set; } = null!;
    
    public string? Key { get; set; }
    
    public UserJobInfo Validate()
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
    private readonly UserJobInfo _userJob = new();

    public JobInfoBuilder<T> SetJob(Expression<Func<T, SimpleSchedulerJobContext, Task>> selector)
        => LambdaReturn(() =>
        {
            if (selector is LambdaExpression { Body: MethodCallExpression methodCallExpression })
            {
                _userJob.MethodName = methodCallExpression.Method.Name;
                _userJob.Type = typeof(T).FullName!;
                return;
            }
            throw new NotSupportedException("Expression is not supported by simple scheduler.");
        });
        
    
    public JobInfoBuilder<T> SetKey(string key)
        => LambdaReturn(() => _userJob.Key = key);
    
    public UserJobInfo Build()
        => _userJob;
    
    private JobInfoBuilder<T> LambdaReturn(Action action)
    {
        action();
        return this;
    }
}
