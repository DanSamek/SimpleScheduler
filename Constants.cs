namespace SimpleScheduler;

internal static class Constants
{
    /// <summary>
    /// Size of the page size for views.
    /// </summary>
    public const int PAGE_SIZE = 32;
    
    /// <summary>
    /// Cookie of the logged user. 
    /// </summary>
    public const string USER_COOKIE = "SIMPLE-SCHEDULER-COOKIE";
    
    /// <summary>
    /// Token expiration time.
    /// </summary>
    public static readonly TimeSpan TOKEN_EXPIRATION_TIME = TimeSpan.FromHours(1);

}