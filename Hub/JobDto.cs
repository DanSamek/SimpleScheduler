namespace SimpleScheduler.Hub;

/// <summary>
/// Dto for the job used in the scheduler hub
/// </summary>
public record JobDto(int Id, string Key, string State, DateTime ExecutionTime);