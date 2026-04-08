using Microsoft.EntityFrameworkCore;

namespace SimpleScheduler.ContextProvider;

internal class DbContextProvider
{
    private readonly Type _dbContextType;
    private readonly IServiceScopeFactory  _scopeFactory;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public DbContextProvider(Type dbContextType, IServiceScopeFactory scopeFactory)
    {
        _dbContextType = dbContextType;
        _scopeFactory = scopeFactory;
    }
    
    /// <summary>
    /// Creates scope for the DbContext.
    /// </summary>
    public async Task<T> WithContext<T>(Func<DbContext, Task<T>> action)
    {
        using var scope = _scopeFactory.CreateScope();
        await using var context = scope.GetSchedulerContext(_dbContextType);
        var result = await action(context);
        return result;
    }
    
    /// <summary>
    /// Creates scope for the DbContext.
    /// </summary>
    public async Task WithContext(Func<DbContext, Task> action)
    {
        using var scope = _scopeFactory.CreateScope();
        await using var context = scope.GetSchedulerContext(_dbContextType);
        await action(context);
        // TODO maybe add SaveChanges.
    }
}

public static class ServiceScopeExtensions
{
    public static DbContext GetSchedulerContext(this IServiceScope scope, Type dbContextType)
    {
        var instance = scope.ServiceProvider.GetRequiredService(dbContextType);
        return (instance as DbContext)!;
    }
}