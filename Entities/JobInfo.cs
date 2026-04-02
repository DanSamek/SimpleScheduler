namespace SimpleScheduler.Entities;


public class JobInfo : DoId
{
    /// <summary>
    /// .Ctor
    /// </summary>
    public JobInfo(UserJobInfo userJobInfo)
    {
        Key = userJobInfo.Key;
        Type = userJobInfo.Type;
        MethodName = userJobInfo.MethodName;
    }

    // Ef ctor
    public JobInfo()
    {
        Key = string.Empty;
        Type = string.Empty;
        MethodName = string.Empty;
    }
    
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
    public string Type { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string MethodName { get; set; }
    
    /// <summary>
    /// 
    /// </summary>
    public string? Key { get; set; }
}
