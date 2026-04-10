using System.Reflection;
using SimpleScheduler.Entities.Db;

namespace SimpleScheduler.Mapper;

internal record ExecutionWithJob(Execution Execution, MethodInfo MethodInfo, object? Object, object?[]? Arguments);