using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleScheduler.Hub;

namespace SimpleScheduler.Views.SimpleScheduler;

public class JobModel : PageModel
{
    public required JobDto JobDto { get; set; }
}