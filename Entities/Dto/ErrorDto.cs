namespace SimpleScheduler.Entities.Dto;

public record ErrorDto(int Id, int ExecutionId, string Message, string Occurred);