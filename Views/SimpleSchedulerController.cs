using Microsoft.AspNetCore.Mvc;
using SimpleScheduler.Storage;
using Index = SimpleScheduler.Views.SimpleScheduler.Index;

namespace SimpleScheduler.Views;

public class SimpleSchedulerController : Controller
{
    private readonly IStorage _storage;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public SimpleSchedulerController(IStorage storage)
    {
        _storage = storage;
    }
    
    [HttpGet("/simple-scheduler")]
    public IActionResult Index()
    {
        var result = _storage.AllJobs();
        var model = new Index
        {
            Jobs = result
        };
        return View(model);
    }
}