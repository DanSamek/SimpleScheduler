using SimpleScheduler.ContextProvider;
using SimpleScheduler.Hub;
using SimpleScheduler.Mapper;
using SimpleScheduler.Middlewares;
using SimpleScheduler.Services;

namespace SimpleScheduler;

public static class SimpleScheduler
{
    /// <summary>
    /// Adds simple scheduler to the services.
    /// </summary>
    public static void AddSimpleScheduler(this IServiceCollection services, Action<SimpleSchedulerOptions> optionsAction)
    {
        var options = new SimpleSchedulerOptions();
        optionsAction.Invoke(options);
        options.Validate();
        
        services.AddSingleton<SimpleSchedulerOptions>(_ => options);
        services.AddSingleton<SchedulerHubNotifier>();
        services.AddSingleton<IStorage, EfStorage>();
        services.AddSingleton<ThreadPool.ThreadPool>(_ => new ThreadPool.ThreadPool(options.NumberOfThreads));
        services.AddSingleton<IJobMapper, JobMapper>();
        services.AddSingleton<Scheduler.Scheduler>();
        services.AddSingleton<SimpleSchedulerMiddleware>();
        services.AddSingleton<DbContextProvider>(sp =>
        {
            var scopeFactory = sp.GetService<IServiceScopeFactory>();
            var instance = new DbContextProvider(options.DbContextType!, scopeFactory!);
            return instance;
        });
        services.AddSingleton<ITokenService, TokenService>();
        
        services
            .AddRazorPages()
            .AddApplicationPart(typeof(SimpleScheduler).Assembly);
        
        services.AddSignalR();
    }
    
    /// <summary>
    /// Runs scheduler in the background.
    /// </summary>
    public static void UseSimpleScheduler(this WebApplication app)
    {
        var services = app.Services;
        var threadPool = services.GetService<ThreadPool.ThreadPool>()!;
        var scheduler = services.GetService<Scheduler.Scheduler>()!;
        var storage = services.GetService<IStorage>()!;
        
        Jobs.SetStorage(storage);
        
        threadPool.Run();
        scheduler.Run();
        
        app.UseWhen(
            context => context.Request.Path.StartsWithSegments("/simple-scheduler"),
            branch =>
            {
                branch.UseMiddleware<SimpleSchedulerMiddleware>();
            });
        
        app.UseRouting();

        app.UseStaticFiles();
        
        app.MapHub<SchedulerHub>("/simple-scheduler-hub");
        app.MapRazorPages()
            .WithStaticAssets();

        app.MapControllers();
    }
}