namespace SimpleScheduler.Jobs;

internal interface IValidatable<out T>
{
    T Validate();
}