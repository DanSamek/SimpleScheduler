namespace SimpleScheduler.Entities.Dto;

public record ErrorDto(int ExecutionId, string Message, DateTime Occurred);