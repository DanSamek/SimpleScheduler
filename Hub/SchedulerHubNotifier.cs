using Microsoft.AspNetCore.SignalR;
using SimpleScheduler.Entities;

namespace SimpleScheduler.Hub;

public class SchedulerHubNotifier
{
    private readonly IHubContext<SchedulerHub> _hubContext;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public SchedulerHubNotifier(IHubContext<SchedulerHub>  hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyClients(Job job)
    {
        var dto = new JobDto(job.Key, job.State, job.ExecutionTime);
        await _hubContext.Clients.All.SendAsync("JobUpdate", dto);
    }
}