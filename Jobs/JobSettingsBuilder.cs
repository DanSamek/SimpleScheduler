using System.Text.Json;

namespace SimpleScheduler.Jobs;


public class JobSettingsBuilder
{
    private readonly UserJobSettings _userJobSettings = new();
    
    public UserJobSettings Build()
        => _userJobSettings;

    public JobSettingsBuilder SetRecurrence(TimeSpan recurrence)
        => LambdaReturn(() => _userJobSettings.Recurrence = recurrence);

    public JobSettingsBuilder SetDelay(TimeSpan delay)
        => LambdaReturn(() => _userJobSettings.Delay = delay);

    public JobSettingsBuilder SetRetrySchedule(params TimeSpan[] args)
        => LambdaReturn(() => _userJobSettings.Retries = args);
    
    public JobSettingsBuilder SetRetrySchedule(TimeSpan retryTime, int count)
        => SetRetrySchedule(Enumerable.Range(0, count).Select(_ => retryTime).ToArray());
    
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