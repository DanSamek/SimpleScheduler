using SimpleScheduler.Entities;

namespace SimpleScheduler.Hub;

/// <summary>
/// Dto for the job used in the scheduler hub
/// </summary>
public record JobDto(string Key, JobState State, DateTime ExecutionTime);