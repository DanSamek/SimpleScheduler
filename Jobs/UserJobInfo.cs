namespace SimpleScheduler.Jobs;


public class UserJobInfo : IValidatable<UserJobInfo>
{
    public string Type { get; set; } = null!;

    public string MethodName { get; set; } = null!;
    
    public string? Key { get; set; }
    
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
