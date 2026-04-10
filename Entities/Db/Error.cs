using SimpleScheduler.Entities.Dto;

namespace SimpleScheduler.Entities.Db;

public class Error : DoId, IDto<ErrorDto>
{
    /// <summary>
    /// Message of the error, that happened.
    /// </summary>
    public required string ErrorMessage { get; set; }
    
    /// <summary>
    /// Execution, where occured the error.
    /// </summary>
    public Execution? Execution { get; set; }
    
    /// <summary>
    /// Id of the <see cref="Execution"/>.
    /// </summary>
    public int ExecutionId { get; set; }
    
    /// <summary>
    /// Time, when error occured.
    /// </summary>
    public required DateTime Occurred { get; set; }

    /// <inheritdoc /> 
    public ErrorDto ToDto(int recursionDepth)
    {
        var instance = new ErrorDto(ExecutionId, ErrorMessage,  Occurred);
        return instance;
    }
}