using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Views.SimpleScheduler;

public class Executions : PageModel
{
    public required List<ExecutionDto> ExecutionsList { get; set; }
}