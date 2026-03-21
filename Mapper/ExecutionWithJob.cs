using SimpleScheduler.Entities;

namespace SimpleScheduler.Mapper;

public record ExecutionWithJob(Execution Execution, Func<Task>? Job);