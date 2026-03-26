using SimpleScheduler.Hub;
using SimpleScheduler.Mapper;
using SimpleScheduler.Middlewares;
using SimpleScheduler.Storage;

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
        
        services.AddSingleton<SimpleSchedulerUser>(_ => options.User!);
        services.AddSingleton<SchedulerHubNotifier>();
        services.AddSingleton<IStorage>(sp =>
        {
            var storageType = typeof(EfStorage<>).MakeGenericType(options.DbContextType!);
            var instance = (IStorage)ActivatorUtilities.CreateInstance(sp, storageType);
            return instance;
        });
        services.AddSingleton<ThreadPool.ThreadPool>(_ => new ThreadPool.ThreadPool(options.NumberOfThreads));
        services.AddSingleton<IJobMapper, JobMapper>();
        services.AddSingleton<Scheduler>();
        services.AddSingleton<SimpleSchedulerMiddleware>();
        
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
        var scheduler = services.GetService<Scheduler>()!;
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