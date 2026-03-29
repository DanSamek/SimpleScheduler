using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Views.SimpleScheduler;

public class IndexModel : PageModel
{
    public required SimpleSchedulerOptions Options { get; set; }
    public required List<ExecutionDto> RunningExecutions { get; set; }
}