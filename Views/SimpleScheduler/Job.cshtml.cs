using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleScheduler.Entities.Dto;

namespace SimpleScheduler.Views.SimpleScheduler;

public class JobModel : PageModel
{
    public required JobDto JobDto { get; set; }
}