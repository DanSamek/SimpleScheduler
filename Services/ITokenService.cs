namespace SimpleScheduler.Services;

public interface ITokenService
{
    /// <summary>
    /// Removes all expired tokens.
    /// </summary>
    Task RemoveExpiredTokens();

    /// <summary>
    /// Validates a token.
    /// </summary>
    Task<bool> ValidateToken(string token);
    
    /// <summary>
    /// Adds token with value and expiration.
    /// </summary>
    Task AddToken(string value, TimeSpan expiration);
}