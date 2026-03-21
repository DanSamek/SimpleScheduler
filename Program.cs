using Microsoft.EntityFrameworkCore;
using SimpleScheduler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SimpleSchedulerContext>(dbOptions => dbOptions.UseInMemoryDatabase("SimpleScheduler"));

builder.Services.AddSimpleScheduler(options =>
{
    options.NumberOfThreads = 4;
    options.DbContextType = typeof(SimpleSchedulerContext);
});

var app = builder.Build();

app.UseSimpleScheduler();

Jobs.AddRecurringJob(() =>
{
    Console.WriteLine("[START] Some recurrent running job");
    Thread.Sleep(10000);
    Console.WriteLine("[END] Some recurrent running job");
    return Task.CompletedTask;
}, TimeSpan.FromSeconds(60));


app.Run();