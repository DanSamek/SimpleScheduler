using System.Linq.Expressions;
using SimpleScheduler.Scheduler;

namespace SimpleScheduler.Jobs;

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
