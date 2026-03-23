using Microsoft.AspNetCore.Mvc;
using SimpleScheduler.Storage;
using SimpleScheduler.Views.SimpleScheduler;
using Index = SimpleScheduler.Views.SimpleScheduler.Index;

namespace SimpleScheduler.Views;

public class SimpleSchedulerController : Controller
{
    private readonly IStorage _storage;
    private readonly Scheduler _scheduler;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public SimpleSchedulerController(IStorage storage, Scheduler scheduler)
    {
        _storage = storage;
        _scheduler = scheduler;
    }
    
    [HttpGet("/simple-scheduler")]
    public IActionResult Index()
    {
        var model = new Index();
        return View(model);
    }
    
    [HttpGet("/simple-scheduler/executions")]
    public async Task<IActionResult> Executions([FromQuery] int? pageId)
    {
        var result = await _storage.ExecutionsPage(pageId ?? 0);
        var totalPages = await _storage.TotalExecutionPages();
        var totalExecutions = await _storage.TotalExecutions();
        var model = new Executions
        {
            ExecutionsList = result
                .Select(e => e.ToDto())
                .ToList(),
            TotalPages = totalPages,
            PageIndex = pageId ?? 0,
            TotalExecutions = totalExecutions
        };
        return View(model);
    }

    [HttpGet("/simple-scheduler/executions/{id:int}")]
    public async Task<IActionResult> Execution(int id)
    {
        var execution = await _storage.ExecutionById(id);
        if (execution == null)
        {
            return NotFound();
        }
        
        var model = new Execution
        {
            ExecutionDto = execution.ToDto()
        };
        return View(model);
    }
     
    [HttpGet("/simple-scheduler/jobs")]
    public async Task<IActionResult> Jobs()
    {
        var jobs = await _storage.AllJobs();
        var model = new SimpleScheduler.Jobs
        {
            JobItems = jobs.Select(e => e.ToDto()).ToList()
        };
        return View(model);
    }
    [HttpPost("/simple-scheduler/jobs/schedule/{id:int}")]
    public async Task<IActionResult> Schedule(int id)
    {
        var result = await _scheduler.ScheduleJob(id);
        return result ? Ok() : NotFound();
    }
}