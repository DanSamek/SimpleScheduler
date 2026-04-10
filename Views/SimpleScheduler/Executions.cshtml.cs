using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleScheduler.Entities.Dto;

namespace SimpleScheduler.Views.SimpleScheduler;

public class ExecutionsModel : PageModel
{
    public required List<ExecutionDto> ExecutionsList { get; set; }
    public required int TotalPages { get; set; }
    public required int TotalExecutions { get; set; }

    public required int PageIndex { get; set; }
}