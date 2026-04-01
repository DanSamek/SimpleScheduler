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
    public static async Task<int> AddInstantJob<T, TData>(Job<T> job, Arguments<TData> arguments)
    {
        var a = JsonSerializer.Serialize(job);
        var b = JsonSerializer.Serialize(arguments);

        var x = typeof(Job<>).MakeGenericType(typeof(T));
        var aa = JsonSerializer.Deserialize(a, x);
        
        var y = typeof(Arguments<>).MakeGenericType(typeof(TData));
        var bb = JsonSerializer.Deserialize(b, y);
        
        await Task.CompletedTask;
        throw new NotImplementedException();
    }
    
    // Old interface below!
    
    /// <summary>
    /// Executes a job once.
    /// </summary>
    public static async Task<int> AddInstantJob<T>(Expression<Func<T, Task>> job, TimeSpan? delay = null, string? key = null)
    {
        return await AddJob(job, delay: delay, key: key);
    }

    /// <summary>
    /// Executes a job repeatedly.
    /// </summary>
    public static async Task<int> AddRecurringJob<T>(Expression<Func<T, Task>> job, TimeSpan recurrence, TimeSpan? delay = null, string? key = null)
    {
        return await AddJob(job, recurrence, delay, key);
    }

    private static async Task<int> AddJob<T>(Expression<Func<T, Task>> job, TimeSpan? recurrence = null, TimeSpan? delay = null, string? key = null)
    {
        if (job.Body is not MethodCallExpression methodCall)
        {
            throw new NotSupportedException("Expression is not supported, only lambda method call is supported.");
        }
        var fullName = typeof(T).FullName;
        if (fullName == null)
        {
            throw new NullReferenceException("Type name is null");
        }

        var call = $"{fullName}.{job.Body.ToString().Split(".").Last()};";
        
        var arguments = ParseArguments(methodCall.Arguments)
            .Select(a => a.Flatten())
            .ToList();
        
        var methodName = methodCall.Method.Name;
        var instance = new Job(fullName, methodName, arguments, call, key, recurrence, delay);
        var result = await _storage.AddJob(instance);
        return result.Id;
    }

    private static List<Argument> ParseArguments(IEnumerable<Expression> arguments)
    {
        var result = arguments
            .Select(ParseArgument)
            .ToList();
        
        return result;
    }

    private static Argument ParseArgument(Expression expression)
    {
        var instance = new Argument();
        switch (expression)
        {
            case ConstantExpression constantExpression:
                instance.Type = constantExpression.Type.FullName!;
                instance.Value = JsonSerializer.Serialize(constantExpression.Value);
                return instance;
            case NewExpression newExpression:
                instance.Type = newExpression.Type.FullName!;
                instance.Arguments = newExpression.Arguments
                    .Select(ParseArgument)
                    .ToList();
                instance.ArgumentCount = instance.Arguments.Count;
                return instance;
            default:
                throw new NotSupportedException("Expression is not supported by simple scheduler.");
        }
    }
    
}

public interface IValidatable<out T>
{
    T Validate();
}

public class Arguments
{
    public TimeSpan Recurrence { get; set; }
    
    public TimeSpan Delay { get; set; }

    public RetrySchedule RetrySchedule { get; set; } = new();
}


public class Arguments<T> : Arguments, IValidatable<Arguments<T>>
{
    public T? Data { get; set; }

    public Arguments<T> Validate()
    {
        return Data == null ? throw new NullReferenceException("Data is null") : this;
    }
}

public class RetrySchedule
{
    public TimeSpan[] Retries { get; set; } = [];
}


public class ArgumentBuilder<T>
{
    private readonly Arguments<T> _arguments = new();

    public ArgumentBuilder<T> SetData(T data)
    {
        _arguments.Data = data;
        return this;
    }
    
    public Arguments<T> Build()
        => _arguments.Validate();

    public ArgumentBuilder<T> SetRecurrence(TimeSpan recurrence)
        => LambdaReturn(() => _arguments.Recurrence = recurrence);

    public ArgumentBuilder<T> SetDelay(TimeSpan delay)
        => LambdaReturn(() => _arguments.Delay = delay);

    public ArgumentBuilder<T> SetRetrySchedule(params TimeSpan[] args)
        => LambdaReturn(() => _arguments.RetrySchedule.Retries = args);
    
    public ArgumentBuilder<T> SetRetrySchedule(TimeSpan retryTime, int count)
        => SetRetrySchedule(Enumerable.Range(0, count).Select(_ => retryTime).ToArray());

    private ArgumentBuilder<T> LambdaReturn(Action action)
    {
        action();
        return this;
    }
}

public class ArgumentBuilder
{
    private readonly Arguments _arguments = new();
    
    public Arguments Build()
        => _arguments;

    public ArgumentBuilder SetRecurrence(TimeSpan recurrence)
        => LambdaReturn(() => _arguments.Recurrence = recurrence);

    public ArgumentBuilder SetDelay(TimeSpan delay)
        => LambdaReturn(() => _arguments.Delay = delay);

    public ArgumentBuilder SetRetrySchedule(params TimeSpan[] args)
        => LambdaReturn(() => _arguments.RetrySchedule.Retries = args);
    
    public ArgumentBuilder SetRetrySchedule(TimeSpan retryTime, int count)
        => SetRetrySchedule(Enumerable.Range(0, count).Select(_ => retryTime).ToArray());

    private ArgumentBuilder LambdaReturn(Action action)
    {
        action();
        return this;
    }
}


public class Job<T> : IValidatable<Job<T>>
{
    public string? Name { get; set; }
    
    public string? Key { get; set; }
    
    public Job<T> Validate()
    {
        return Name == null ? throw new NullReferenceException("Name is null") : this;
    }
}

public class SimpleSchedulerJobContext
{
    
}

public class JobBuilder<T>
{
    private Job<T> _job = new();

    public JobBuilder<T> SetJob(Expression<Func<T, SimpleSchedulerJobContext, Task>> selector)
        => LambdaReturn(() =>
        {
            if (selector is LambdaExpression { Body: MethodCallExpression methodCallExpression })
            {
                _job.Name = methodCallExpression.Method.Name;
                return;
            }
            throw new NotSupportedException("Expression is not supported by simple scheduler.");
        });
        
    
    public JobBuilder<T> SetKey(string key)
        => LambdaReturn(() => _job.Key = key);
    
    public Job<T> Build()
        => _job;
    
    private JobBuilder<T> LambdaReturn(Action action)
    {
        action();
        return this;
    }
}
