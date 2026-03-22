using Microsoft.EntityFrameworkCore;
using SimpleScheduler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SimpleSchedulerContext>(dbOptions => dbOptions.UseInMemoryDatabase("SimpleScheduler"));

builder.Services.AddSimpleScheduler(options =>
{
    options.NumberOfThreads = 2;
    options.DbContextType = typeof(SimpleSchedulerContext);
});

// Force to the DI container.
builder.Services.AddTransient<Test>();

var app = builder.Build();

app.UseSimpleScheduler();

Jobs.AddRecurringJob<Test>(t => t.Run(), TimeSpan.FromSeconds(10));
Jobs.AddRecurringJob<Test>(t => t.Run2(), TimeSpan.FromSeconds(10));
Jobs.AddRecurringJob<Test>(t => t.RunException(), TimeSpan.FromSeconds(10));
Jobs.AddInstantJob<Test>(t => t.LongRunningJob());
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
        throw new Exception();
    }

    public async Task LongRunningJob()
    {
        await Task.Delay(TimeSpan.FromMinutes(1));
    }
}