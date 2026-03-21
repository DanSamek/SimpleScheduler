using Microsoft.AspNetCore.Mvc;
using SimpleScheduler.Hub;
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
        var result = _storage.AllExecutions();
        var model = new Index
        {
            ExecutionInfos = result.Select(e => e.AsDto()).ToList()
        };
        return View(model);
    }
}