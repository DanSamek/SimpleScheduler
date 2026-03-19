using SimpleScheduler;
using ThreadPool = SimpleScheduler.ThreadPool;

var configuration = new SchedulerConfiguration
{
    NumberOfThreads = 2
};

var storage = new InMemoryStorage();
Jobs.Storage = storage;

var threadPool = new ThreadPool(configuration.NumberOfThreads);
var scheduler = new Schleduler(threadPool, storage);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
threadPool.Run();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
scheduler.Run();
await Task.Delay(TimeSpan.FromSeconds(5));

Jobs.AddInstantJob(() =>
{
    Console.WriteLine("Some instant job");
    return Task.CompletedTask;
});

Jobs.AddInstantJob(() =>
{
    Console.WriteLine("Some instant job 2");
    return Task.CompletedTask;
}, TimeSpan.FromSeconds(10));


Jobs.AddRecurringJob(() =>
{
    Console.WriteLine("Some recurring job");
    return Task.CompletedTask;
}, TimeSpan.FromSeconds(10));


await Task.Delay(TimeSpan.FromSeconds(200));