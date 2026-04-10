namespace SimpleScheduler.Entities;

internal interface IDto<out T>
{
    /// <summary>
    /// Converts entity to the dto.
    /// </summary>
    public T? ToDto(int recursionDepth);
}