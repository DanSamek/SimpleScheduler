using Microsoft.EntityFrameworkCore;
using SimpleScheduler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SimpleSchedulerContext>(dbOptions => dbOptions.UseInMemoryDatabase("SimpleScheduler"));

builder.Services.AddSimpleScheduler(options =>
{
    options.NumberOfThreads = 2;
    options.DbContextType = typeof(SimpleSchedulerContext);
    options.User = new SimpleSchedulerUser
    {
        Username = "user",
        Password = "12345678"
    };
    options.RetryCount = 5;
});

// Force to the DI container.
builder.Services.AddTransient<Test>();

var app = builder.Build();

app.UseSimpleScheduler();

await Jobs.AddInstantJob(
    new JobInfoBuilder<Test>()
        .SetJob((t, c) => t.Job(c))
        .SetKey("INSANE_JOB")
        .Build(),
    new ArgumentBuilder()
        .SetData(new Test.EntityB(new Test.EntityA(1,2,3), 4))
        .SetRecurrence(TimeSpan.FromHours(24))
        .SetDelay(TimeSpan.FromHours(1))
        .SetRetrySchedule(TimeSpan.FromMinutes(5), 10)
        .Build()
);

app.Run();

class Test
{
    public async Task Job(SimpleSchedulerJobContext c)
    {
        var data = c.GetData<int>();
        await Task.Delay(data * 5);
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