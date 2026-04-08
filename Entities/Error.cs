namespace SimpleScheduler.Entities;

internal class Error : DoId
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
}