namespace SimpleScheduler.Entities;

public class Argument : DoId
{
    /// <summary>
    /// Type of the class in the job.
    /// </summary>
    public string Type { get; set; } = null!;
    
    /// <summary>
    /// Value of the argument (can be null, if argument has child arguments).
    /// </summary>
    public object? Value { get; set; } = null!;
    
    /// <summary>
    /// All nested arguments of the argument.
    /// </summary>
    public List<Argument> Arguments { get; set; } = [];
    
    /// <summary>
    /// Total number of child arguments.
    /// NOTE: for root argument it's not used.
    /// </summary>
    public int ArgumentCount { get; set; }
    
    /// <summary>
    /// Starting index for child arguments in the flatten array.
    /// NOTE: for root argument it's not used.
    /// </summary>
    public int FlattenChildStartingIndex { get; set; }

    /// <summary>
    /// Flattens all arguments to the one list -- to avoid weird sql queries.
    /// </summary>
    internal Argument Flatten()
    {
        var childStartingIndex = Arguments.Count;
     
        var result = new List<Argument>();
        result.AddRange(Arguments);
        
        var queue = new Queue<Argument>();
        Arguments.ForEach(a => queue.Enqueue(a));
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            current.FlattenChildStartingIndex = childStartingIndex;
            
            childStartingIndex += current.Arguments.Count;
            foreach (var argument in current.Arguments)
            {
                result.Add(argument);
                queue.Enqueue(argument);
            }
            current.Arguments.Clear();
        }
        
        Arguments = result;
        return this;
    }
}