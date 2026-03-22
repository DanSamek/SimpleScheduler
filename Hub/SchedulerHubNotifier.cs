using Microsoft.AspNetCore.SignalR;
using SimpleScheduler.Entities;

namespace SimpleScheduler.Hub;

public class SchedulerHubNotifier
{
    private readonly IHubContext<SchedulerHub> _hubContext;
    
    /// <summary>
    /// .Ctor
    /// </summary>
    public SchedulerHubNotifier(IHubContext<SchedulerHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyClients(Execution info)
    {
        await _hubContext.Clients.All.SendAsync("ExecutionUpdate", info.ToDto());
    }
}