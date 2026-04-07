namespace SimpleScheduler.Jobs;

public class UserJobSettings
{
    public TimeSpan? Recurrence { get; set; }
    
    public TimeSpan? Delay { get; set; }
    
    public TimeSpan[] Retries { get; set; } = [];
    
    public string? Data { get; set; }

    public string? DataType { get; set; }
}
