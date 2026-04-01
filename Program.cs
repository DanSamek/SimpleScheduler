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

await Jobs.AddInstantJob<Test>(t => t.LongRunningJob());
await Jobs.AddInstantJob(
    new JobBuilder<Test>()
        .SetJob((t, c) => t.Job(c))
        .SetKey("INSANE_JOB")
        .Build(),
    new ArgumentBuilder<Test.EntityB>()
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
        await Task.Delay(5000);
    }
    
    public async Task Run()
    {
        await Task.Delay(5000);
    }

    public async Task Run2()
    {
        await Task.Delay(5000);
    }

    public async Task RunException()
    {
        await Task.Delay(5000);
        throw new NullReferenceException();
    }

    public async Task LongRunningJob()
    {
        await Task.Delay(TimeSpan.FromMinutes(1));
    }
    
    public async Task WithArguments(int a, int b)
    {
        await Task.Delay(TimeSpan.FromSeconds(a + b));
    }

    public async Task WithDateTime(DateTime dt)
    {
        await Task.CompletedTask;
    }

    public record EntityA(int a, int b, int c);
    public record EntityB(EntityA t2d, int b);

    public async Task WithComplexArgument(EntityB test)
    {
        await Task.Delay(TimeSpan.FromSeconds(test.t2d.a + test.b));
    }
}