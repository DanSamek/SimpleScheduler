using Microsoft.EntityFrameworkCore;
using SimpleScheduler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SimpleSchedulerContext>(dbOptions => dbOptions.UseInMemoryDatabase("SimpleScheduler"));

builder.Services.AddSimpleScheduler(options =>
{
    options.NumberOfThreads = 1;
    options.DbContextType = typeof(SimpleSchedulerContext);
});

// Force to the DI container.
builder.Services.AddTransient<Test>();

var app = builder.Build();

app.UseSimpleScheduler();

Jobs.AddInstantJob<Test>(t => t.Run(), TimeSpan.FromSeconds(10), key: "test-run-instant");
Jobs.AddInstantJob<Test>(t => t.Run(), TimeSpan.FromSeconds(10), key: "test-run-instant-2");
Jobs.AddInstantJob<Test>(t => t.Run(), TimeSpan.FromSeconds(10), key: "test-run-instant-3");
Jobs.AddRecurringJob<Test>(t => t.Run(), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), "test-run-recurring");

app.Run();

class Test
{
    public async Task Run()
    {
        await Task.Delay(5000);
    }
}