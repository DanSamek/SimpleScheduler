using Microsoft.AspNetCore.Mvc;
using SimpleScheduler.Storage;
using SimpleScheduler.Views.SimpleScheduler;
namespace SimpleScheduler.Controllers;

public class SimpleSchedulerController : Controller
{
    private readonly IStorage _storage;
    private readonly Scheduler.Scheduler _scheduler;
    private readonly SimpleSchedulerUser _user;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public SimpleSchedulerController(IStorage storage, Scheduler.Scheduler scheduler, SimpleSchedulerUser user)
    {
        _storage = storage;
        _scheduler = scheduler;
        _user = user;
    }

    [HttpGet("/simple-scheduler/login")]
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost("/simple-scheduler/login")]
    public IActionResult Login([FromForm] string username, [FromForm] string password)
    {
        if (_user.Username != username || _user.Password != password)
        {
            // TODO error message
            return View("Login");
        }
        
        // TODO add auth tokens instead of "_user.Username".
        HttpContext.Response.Cookies.Append(Constants.USER_COOKIE, _user.Username, new CookieOptions
        {
            MaxAge = TimeSpan.FromHours(1)
        });
        
        return Redirect("/simple-scheduler");
    }
    
    [HttpGet("/simple-scheduler")]
    public IActionResult Index()
    {
        var model = new IndexModel();
        return View(model);
    }
    
    [HttpGet("/simple-scheduler/executions")]
    public async Task<IActionResult> Executions([FromQuery] int? pageId)
    {
        var result = await _storage.ExecutionsPage(pageId ?? 0);
        var totalPages = await _storage.TotalExecutionPages();
        var totalExecutions = await _storage.TotalExecutions();
        var model = new ExecutionsModel
        {
            ExecutionsList = result
                .Select(e => e.ToDto(2))
                .ToList()!,
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
        
        var model = new ExecutionModel
        {
            ExecutionDto = execution.ToDto(2)!
        };
        return View(model);
    }
     
    [HttpGet("/simple-scheduler/jobs")]
    public async Task<IActionResult> Jobs()
    {
        var jobs = await _storage.AllJobs();
        var model = new JobsModel
        {
            JobItems = jobs.Select(e => e.ToDto(2)).ToList()!
        };
        return View(model);
    }

    [HttpGet("/simple-scheduler/jobs/{id:int}")]
    public async Task<IActionResult> Job(int id)
    {
        var job = await _storage.JobById(id);
        if (job == null)
        {
            return NotFound();
        }
        
        var model = new JobModel
        {
            JobDto = job.ToDto(2)!
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