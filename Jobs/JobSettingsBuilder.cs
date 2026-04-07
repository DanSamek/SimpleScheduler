using System.Text.Json;

namespace SimpleScheduler.Jobs;

/// <summary>
/// Builder for <see cref="UserJobSettings"/>.
/// </summary>
public class JobSettingsBuilder
{
    private readonly UserJobSettings _userJobSettings = new();
    
    /// <summary>
    /// Builds the instance.
    /// </summary>
    public UserJobSettings Build()
        => _userJobSettings;
    
    /// <summary>
    /// Sets the recurrence interval for the job.
    /// </summary>
    /// <param name="recurrence">The time interval between job executions.</param>
    public JobSettingsBuilder SetRecurrence(TimeSpan recurrence)
        => LambdaReturn(() => _userJobSettings.Recurrence = recurrence);

    /// <summary>
    /// Sets the initial delay before the job is first executed.
    /// </summary>
    /// <param name="delay">The delay before the first execution.</param>
    public JobSettingsBuilder SetDelay(TimeSpan delay)
        => LambdaReturn(() => _userJobSettings.Delay = delay);

    /// <summary>
    /// Sets the retry schedule using a collection of retry delays.
    /// </summary>
    /// <param name="args">An array of time intervals representing delays between retry attempts.</param>
    public JobSettingsBuilder SetRetrySchedule(params TimeSpan[] args)
        => LambdaReturn(() => _userJobSettings.Retries = args);
    
    /// <summary>
    /// Sets a retry schedule with a fixed retry interval repeated a specified number of times.
    /// </summary>
    /// <param name="retryTime">The delay between each retry attempt.</param>
    /// <param name="count">The number of retry attempts.</param>
    public JobSettingsBuilder SetRetrySchedule(TimeSpan retryTime, int count)
        => SetRetrySchedule(Enumerable.Range(0, count).Select(_ => retryTime).ToArray());
    
    /// <summary>
    /// Sets custom data associated with the job.
    /// </summary>
    /// <param name="data">The data object to associate with the job.</param>
    public JobSettingsBuilder SetData<T>(T data)
    {
        _userJobSettings.DataType = typeof(T).FullName;
        _userJobSettings.Data = JsonSerializer.Serialize(data);
        return this;
    }

    private JobSettingsBuilder LambdaReturn(Action action)
    {
        action();
        return this;
    }
}