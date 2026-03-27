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
});

// Force to the DI container.
builder.Services.AddTransient<Test>();

var app = builder.Build();

app.UseSimpleScheduler();

await Jobs.AddInstantJob<Test>(t => t.WithDateTime(new DateTime(1,2,3)));
await Jobs.AddInstantJob<Test>(t => t.WithComplexArgument(new Test.EntityB(new Test.EntityA(1, 2, 3), 2)));
await Jobs.AddInstantJob<Test>(t => t.WithComplexArgument(new Test.EntityB(new Test.EntityA(1, 2, 3), 2)));
await Jobs.AddInstantJob<Test>(t => t.WithArguments(2, 3));
await Jobs.AddRecurringJob<Test>(t => t.Run(), TimeSpan.FromSeconds(10));
await Jobs.AddRecurringJob<Test>(t => t.Run2(), TimeSpan.FromSeconds(10));
await Jobs.AddRecurringJob<Test>(t => t.RunException(), TimeSpan.FromSeconds(10));
await Jobs.AddInstantJob<Test>(t => t.LongRunningJob());
app.Run();

class Test
{
    public async Task Run()
    {
        await Task.Delay(5000);
    }

    public async Task Run2()
    {
        await Task.Delay(5000);
    }

    public Task RunException()
    {
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