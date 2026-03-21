namespace SimpleScheduler.Hub;

/// <summary>
/// Dto for the job used in the scheduler hub
/// </summary>
public record ExecutionDto(int Id, string Key, string State, string ExecutionTime);