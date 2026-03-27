using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Views.SimpleScheduler;

public class ExecutionModel : PageModel
{
    public required ExecutionDto ExecutionDto { get; set; }
}