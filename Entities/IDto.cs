namespace SimpleScheduler.Entities;

public interface IDto<out T>
{
    /// <summary>
    /// Converts entity to the dto.
    /// </summary>
    public T? ToDto(int recursionDepth);
}