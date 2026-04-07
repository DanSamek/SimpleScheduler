namespace SimpleScheduler.Jobs;


/// <summary>
/// Represents metadata describing a scheduled job.
/// </summary>
public class UserJobInfo : IValidatable<UserJobInfo>
{
    /// <summary>
    /// The fully qualified name of the type that contains the job method.
    /// </summary>
    internal string Type { get; set; } = null!;

    /// <summary>
    /// The name of the method to be executed.
    /// </summary>
    internal string MethodName { get; set; } = null!;
    
    /// <summary>
    /// An optional custom key that uniquely identifies the job.
    /// </summary>
    internal string? Key { get; set; }
    
    /// <summary>
    /// Validates the current instance to ensure all required properties are set.
    /// </summary>
    /// <returns>The validated <see cref="UserJobInfo"/> instance.</returns>
    /// <exception cref="NullReferenceException">
    /// Thrown when <see cref="Type"/> or <see cref="MethodName"/> is <c>null</c>.
    /// </exception>
    public UserJobInfo Validate()
    {
        if (Type == null)
        {
            throw new NullReferenceException($"{nameof(Type)} name is null");
        }
        if (MethodName == null)
        {
            throw new NullReferenceException($"{nameof(MethodName)} name is null");
        }
        return this;
    }
}
