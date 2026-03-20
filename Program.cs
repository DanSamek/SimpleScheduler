using SimpleScheduler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSimpleScheduler(options =>
{
    options.NumberOfThreads = 4;
});

var app = builder.Build();

app.UseSimpleScheduler();
app.MapGet("/", () => "Simple scheduler");
app.MapGet("/jobs", (SimpleSchedulerContext context) =>
{
    var result = context.Jobs.ToArray();
    return result;  
});

Jobs.AddRecurringJob(() =>
{
    Console.WriteLine("[START] Some recurrent running job");
    Thread.Sleep(10000);
    Console.WriteLine("[END] Some recurrent running job");
    return Task.CompletedTask;
}, TimeSpan.FromSeconds(60));

app.Run();