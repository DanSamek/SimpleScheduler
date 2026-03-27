using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Views.SimpleScheduler;

public class JobsModel : PageModel
{
    public required List<JobDto> JobItems { get; set; }
}