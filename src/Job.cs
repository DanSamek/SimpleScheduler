namespace SimpleScheduler;

public record Job(Func<Task> Work, TimeSpan? Recurrence = null, TimeSpan? Delay = null);