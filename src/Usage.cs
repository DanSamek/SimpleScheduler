using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SimpleScheduler.Hub;
using SimpleScheduler.Mapper;
using SimpleScheduler.Storage;

namespace SimpleScheduler;

public static class Usage
{
    /// <summary>
    /// Adds simple scheduler to the services.
    /// </summary>
    public static void AddSimpleScheduler(this IServiceCollection services, Action<SchedulerOptions> optionsAction)
    {
        var options = new SchedulerOptions();
        optionsAction.Invoke(options);

        services.AddSignalR();
        services.AddDbContext<SimpleSchedulerContext>(dbOptions => dbOptions.UseInMemoryDatabase("SimpleScheduler"));
        services.AddSingleton<SchedulerHubNotifier>();
        services.AddSingleton<IStorage, EfStorage>();
        services.AddSingleton<ThreadPool.ThreadPool>(_ => new ThreadPool.ThreadPool(options.NumberOfThreads));
        services.AddSingleton<IJobMapper, JobMapper>(_ => new JobMapper());

        services.AddSingleton<Scheduler>(sp =>
        {
            var threadPool = sp.GetRequiredService<ThreadPool.ThreadPool>();
            var storage = sp.GetRequiredService<IStorage>();
            var jobMapper = sp.GetRequiredService<IJobMapper>();
            var instance = new Scheduler(threadPool, storage, jobMapper);
            return instance;
        });
    }
    
    /// <summary>
    /// Runs scheduler in the background.
    /// </summary>
    public static void UseSimpleScheduler(this IApplicationBuilder app)
    {
        if (app is IEndpointRouteBuilder erp)
        {
            erp.MapHub<SchedulerHub>("/simple-scheduler");
        }
        
        var services = app.ApplicationServices;
        var threadPool = services.GetService<ThreadPool.ThreadPool>()!;
        var scheduler = services.GetService<Scheduler>()!;
        var mapper = services.GetService<IJobMapper>()!;
        var storage = services.GetService<IStorage>()!;
        
        Jobs.SetJobMapper(mapper);
        Jobs.SetStorage(storage);
        
        threadPool.Run();
        scheduler.Run();
    }
}