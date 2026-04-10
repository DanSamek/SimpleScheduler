namespace SimpleScheduler.Entities.Db;

/// <summary>
/// Token for auth purposes
/// </summary>
internal class Token : DoId
{
    /// <summary>
    /// Value of the token.
    /// </summary>
    public required string Value { get; set; }
    
    /// <summary>
    /// Expiration date of the token.
    /// </summary>
    public required DateTime ExpireDate { get; set; }
}