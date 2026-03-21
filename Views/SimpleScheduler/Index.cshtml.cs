using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleScheduler.Entities;

namespace SimpleScheduler.Views.SimpleScheduler;

public class Index : PageModel
{
    public required List<Job> Jobs { get; set; }
}