using SimpleScheduler.ContextProvider;
using SimpleScheduler.Entities;
using SimpleScheduler.Entities.Db;

namespace SimpleScheduler.Services;

/// <summary>
/// Logger for errors when something happens "inside", there is no way to obtain this information easily.
/// </summary>
internal class ErrorLogger
{
    private readonly DbContextProvider _contextProvider;
    
    /// <summary>
    /// .Ctor 
    /// </summary>
    public ErrorLogger(DbContextProvider contextProvider)
    {
        _contextProvider = contextProvider;
    }
    
    /// <summary>
    /// Adds the error for the <see cref="Execution"/> based on the exception information.
    /// </summary>
    public async Task AddError(Exception exception, int executionId)
    {
        await _contextProvider.WithContext(context =>
        {
            var errorMessage = $"{exception.Message}\n{exception.StackTrace}";
            #if DEBUG
                Console.WriteLine(errorMessage);
            #endif
            context.Set<Error>()
                .Add(new Error
                {
                    ErrorMessage = errorMessage,
                    ExecutionId = executionId,
                    Occurred = DateTime.UtcNow
                });
            context.SaveChanges();
            return Task.CompletedTask;
        });
    }
}