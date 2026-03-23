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

/* TODO !
Jobs.AddInstantJob<Test>(t => t.WithComplexArgument(new Test.TestDto(1, 2)));
Jobs.AddInstantJob<Test>(t => t.WithArguments(2, 3));
*/
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
    
    public record TestDto(int a, int b);

    public async Task WithComplexArgument(TestDto test)
    {
        await Task.Delay(TimeSpan.FromSeconds(test.a + test.b));
    }
}