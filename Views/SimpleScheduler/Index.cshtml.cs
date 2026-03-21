using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Views.SimpleScheduler;

public class Index : PageModel
{
    public required List<ExecutionDto> ExecutionInfos { get; set; }
}