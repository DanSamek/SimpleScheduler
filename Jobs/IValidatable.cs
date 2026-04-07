namespace SimpleScheduler.Jobs;

public interface IValidatable<out T>
{
    T Validate();
}