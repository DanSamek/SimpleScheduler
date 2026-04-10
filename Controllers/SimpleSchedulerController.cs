using Microsoft.AspNetCore.Mvc;
using SimpleScheduler.Entities;
using SimpleScheduler.Services;
using SimpleScheduler.Views.SimpleScheduler;
namespace SimpleScheduler.Controllers;

internal class SimpleSchedulerController : Controller
{
    private readonly IStorage _storage;
    private readonly SimpleSchedulerOptions _options;
    private readonly SimpleSchedulerUser _user;
    private readonly ITokenService _tokenService;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public SimpleSchedulerController(IStorage storage, SimpleSchedulerOptions options, ITokenService tokenService)
    {
        _storage = storage;
        _options = options;
        _user = options.User!;
        _tokenService = tokenService;
    }
    
    [HttpGet("/simple-scheduler/login")]
    public IActionResult Login()
    {
        var model = new LoginModel();
        return View(model);
    }
    
    [HttpPost("/simple-scheduler/login")]
    public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
    {
        if (_user.Username != username || _user.Password != password)
        {
            var model = new  LoginModel
            {
                ErrorMessage = "Invalid credentials"
            };
            return View("Login", model);
        }
        
        var value = Guid.NewGuid().ToString();
        await _tokenService.AddToken(value, Constants.TOKEN_EXPIRATION_TIME);
        HttpContext.Response.Cookies.Append(Constants.USER_COOKIE, value, new CookieOptions
        {
            MaxAge = Constants.TOKEN_EXPIRATION_TIME
        });
        await _tokenService.RemoveExpiredTokens();
        
        return Redirect("/simple-scheduler");
    }
    
    [HttpGet("/simple-scheduler")]
    public async Task<IActionResult> Index()
    {
        var runningExecutions = await _storage.GetExecutionsByState(ExecutionState.Running);
        var model = new IndexModel
        {
            Options = _options,
            RunningExecutions = runningExecutions
                .Select(re => re.ToDto(2))
                .ToList()!
        };
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

    [HttpGet("/simple-scheduler/errors")]
    public async Task<IActionResult> Errors()
    {
        var errors = await _storage.AllErrors();
        var model = new ErrorsModel
        {
            ErrorsDto = errors
                .Select(e => e.ToDto(0))
                .ToList()
        };
        
        return View(model);
    }
}