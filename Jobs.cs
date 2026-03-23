using System.Linq.Expressions;
using SimpleScheduler.Entities;
using SimpleScheduler.Storage;

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
    public static void AddInstantJob<T>(Expression<Func<T, Task>> job, TimeSpan? delay = null, string? key = null)
    {
        AddJob(job, delay: delay, key: key);
    }

    /// <summary>
    /// Executes a job repeatedly.
    /// </summary>
    public static void AddRecurringJob<T>(Expression<Func<T, Task>> job, TimeSpan recurrence, TimeSpan? delay = null, string? key = null)
    {
        AddJob(job, recurrence, delay, key);
    }

    private static void AddJob<T>(Expression<Func<T, Task>> job, TimeSpan? recurrence = null, TimeSpan? delay = null, string? key = null)
    {
        if (job.Body is not MethodCallExpression methodCall || methodCall.Arguments.Count > 0)
        {
            throw new NotSupportedException("Expression is not supported, only lambda method call is supported.");
        }
        var fullName = typeof(T).FullName;
        if (fullName == null)
        {
            throw new NullReferenceException("Type name is null");
        }
        
        var methodName = methodCall.Method.Name;
        var instance = new Job(fullName, methodName, key, recurrence, delay);
        _storage.AddJob(instance);
    }
}