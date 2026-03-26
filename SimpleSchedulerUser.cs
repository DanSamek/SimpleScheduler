namespace SimpleScheduler;

/// <summary>
/// Credentials for login to the simple scheduler.
/// </summary>
public class SimpleSchedulerUser
{
    /// <summary>
    /// Username of the user.
    /// </summary>
    public required string Username { get; set; }
    
    /// <summary>
    /// Plain text password of the user.
    /// </summary>
    public required string Password { get; set; }
    
}