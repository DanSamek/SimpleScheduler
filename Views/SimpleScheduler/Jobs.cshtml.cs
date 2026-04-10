using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleScheduler.Entities.Dto;

namespace SimpleScheduler.Views.SimpleScheduler;

public class JobsModel : PageModel
{
    public required List<JobDto> JobItems { get; set; }
}