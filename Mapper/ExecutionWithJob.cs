using System.Reflection;
using SimpleScheduler.Entities;

namespace SimpleScheduler.Mapper;

public record ExecutionWithJob(Execution Execution, MethodInfo MethodInfo, object? Object, object?[]? Arguments);