using System.Text.Json;
using SimpleScheduler.Jobs;

namespace SimpleScheduler.Entities.Db;

internal class JobSettings : DoId
{
    /// <summary>
    /// .Ctor
    /// </summary>
    public JobSettings(UserJobSettings userJobSettings)
    {
        Data = userJobSettings.Data;
        DataType = userJobSettings.DataType;
        Delay = userJobSettings.Delay;
        Recurrence = userJobSettings.Recurrence;
        Retries = userJobSettings.Retries;
    }
    
    // Ef ctor
    public JobSettings() {}
    
    /// <summary>
    /// 
    /// </summary>
    public int JobId { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public Job? Job { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public TimeSpan? Recurrence { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public TimeSpan? Delay { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public TimeSpan[] Retries { get; set; } = [];
    
    /// <summary>
    /// 
    /// </summary>
    public string? Data { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string? DataType { get; set; }
    
    
    public object? DeserializeData()
    {
        return Data == null ? null : JsonSerializer.Deserialize(Data, Type.GetType(DataType!)!);
    }
}