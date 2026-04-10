using Microsoft.EntityFrameworkCore;
using SimpleScheduler;
using SimpleScheduler.Jobs;
using SimpleScheduler.Scheduler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SimpleSchedulerContext>(dbOptions => dbOptions.UseInMemoryDatabase("SimpleScheduler"));
builder.Services.AddSimpleScheduler(options =>
{
    options.NumberOfThreads = 1;
    options.DbContextType = typeof(SimpleSchedulerContext);
    options.User = new SimpleSchedulerUser
    {
        Username = "user",
        Password = "12345678"
    };
});

// We require the service in the container (to avoid reflection madness).
builder.Services.AddTransient<Test>();

var app = builder.Build();

app.UseSimpleScheduler();

await Jobs.AddJob(
    new JobInfoBuilder<Test>()
        .SetJob((t, c) => t.Job(c))
        .SetKey("INSTANT_JOB")
        .Build(),
    new JobSettingsBuilder()
        .SetData(new Test.EntityB(new Test.EntityA(1,2,3), 4))
        .SetRecurrence(TimeSpan.FromHours(24))
        .SetDelay(TimeSpan.FromMinutes(1))
        .SetRetrySchedule(TimeSpan.FromSeconds(15), 1)
        .Build() 
    );

app.Run();

class Test
{
    public Task Job(SimpleSchedulerJobContext c)
    {
        var data = c.GetDataNotNull<EntityB>();
        Console.WriteLine($"RC: {c.RetryCount} {DateTime.Now}");
        throw new Exception("Test");
    }
    
    public async Task RunException(SimpleSchedulerJobContext c)
    {
        await Task.Delay(5000);
        throw new NullReferenceException();
    }

    public async Task LongRunningJob(SimpleSchedulerJobContext c)
    {
        await Task.Delay(TimeSpan.FromMinutes(1));
    }

    public record EntityA(int a, int b, int c);
    public record EntityB(EntityA t2d, int b);

}