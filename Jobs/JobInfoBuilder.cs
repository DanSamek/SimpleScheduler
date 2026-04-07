using System.Linq.Expressions;
using SimpleScheduler.Scheduler;

namespace SimpleScheduler.Jobs;

/// <summary>
/// Builder for <see cref="UserJobInfo"/>.
/// Simple scheduler expects, that the <see cref="T"/> is in the DI container.
/// </summary>
public class JobInfoBuilder<T>
{
    private readonly UserJobInfo _userJob = new();

    /// <summary>
    /// Sets the method to be executed by the scheduler using a lambda expression.
    /// </summary>
    /// <param name="selector">An expression representing the method to execute.</param>
    /// <exception cref="NotSupportedException">Thrown when the provided expression is not a method call expression.</exception>
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
    
    /// <summary>
    /// Sets a custom key that's identifies the job.
    /// </summary>
    /// <param name="key">The identifier for the job.</param>
    public JobInfoBuilder<T> SetKey(string key)
        => LambdaReturn(() => _userJob.Key = key);
    
    /// <summary>
    /// Builds the instance.
    /// </summary>
    public UserJobInfo Build()
        => _userJob;
    
    private JobInfoBuilder<T> LambdaReturn(Action action)
    {
        action();
        return this;
    }
}
