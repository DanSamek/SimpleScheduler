namespace SimpleScheduler.Entities.Dto;

/// <summary>
/// Dto for the job.
/// </summary>
public record JobDto(int Id, string Key, string Type, string MethodName, 
    bool IsRecurrent, string NextExecutionTime, List<ExecutionDto?>? Executions);