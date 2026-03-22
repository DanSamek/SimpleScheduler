using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Views.SimpleScheduler;

public class Jobs : PageModel
{
    public required List<JobDto> JobItems { get; set; }
}