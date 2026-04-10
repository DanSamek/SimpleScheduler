using SimpleScheduler.ContextProvider;
using SimpleScheduler.Entities.Db;

namespace SimpleScheduler.Services;

internal class TokenService : ITokenService
{
    private readonly DbContextProvider _dbContextProvider;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public TokenService(DbContextProvider dbContextProvider)
    {
        _dbContextProvider = dbContextProvider;
    }
    
    /// <inheritdoc /> 
    public async Task AddToken(string value, TimeSpan expiration)
    {
        await _dbContextProvider.WithContext(context =>
        {
            var expirationDate = DateTime.UtcNow.Add(expiration);
            context.Set<Token>()
                .Add(new Token
                {
                    Value = value,
                    ExpireDate = expirationDate
                });
            context.SaveChanges();
            return Task.CompletedTask;
        });
    }
    
    /// <inheritdoc /> 
    public async Task<bool> ValidateToken(string token)
    {
        return await _dbContextProvider.WithContext(context =>
        {
            var now = DateTime.UtcNow;
            var result = context.Set<Token>()
                .Any(t => t.Value == token && t.ExpireDate >= now);
            
            return Task.FromResult(result);
        });
    }
    
    /// <inheritdoc /> 
    public async Task RemoveExpiredTokens()
    {
        await _dbContextProvider.WithContext(context =>
        {
            var now = DateTime.UtcNow;
            // TODO!! use ExecuteDelete.
            var expiredTokens = context.Set<Token>()
                .Where(t => t.ExpireDate < now);

            context.Set<Token>().RemoveRange(expiredTokens);
            context.SaveChanges();

            return Task.CompletedTask;
        });
    }
}
