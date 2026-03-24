using System.Reflection;
using System.Text.Json;

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
    public string? Value { get; set; }
    
    /// <summary>
    /// All nested arguments of the argument.
    /// NOTE, its should be cleared because we have <see cref="Flatten"/>.
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
    /// If argument is in the "root"
    /// </summary>
    public bool FlattenRoot { get; set; }
    
    /// <summary>
    /// Flattens all arguments to the one list -- to avoid weird sql queries.
    /// </summary>
    internal Argument Flatten()
    {
        // Initialize root.
        FlattenChildStartingIndex = 0;
        ArgumentCount = Arguments.Count;
        
        var childStartingIndex = Arguments.Count;
        var result = new List<Argument>();
        result.AddRange(Arguments);
        
        var queue = new Queue<Argument>();
        Arguments.ForEach(a =>
        {
            a.FlattenRoot = true;
            queue.Enqueue(a);
        });
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

    /// <summary>
    /// Creates instance of the argument.
    /// </summary>
    internal object CreateInstance()
    {
        var result = CreateInstanceRec(this);
        return result;
    }

    private object CreateInstanceRec(Argument argument)
    {
        var argumentType = System.Type.GetType(argument.Type)!;
        if (argument.ArgumentCount == 0)
        {
            var deserializedValue = JsonSerializer.Deserialize(argument.Value!, argumentType);
            return deserializedValue!;
        }
        
        var instanceArguments = new List<Type>();
        for (var i = argument.FlattenChildStartingIndex; i < argument.FlattenChildStartingIndex + argument.ArgumentCount; i++)
        {
            instanceArguments.Add(System.Type.GetType(Arguments[i].Type)!);
        }
        
        var suitableConstructor = GetSuitableConstructor(argumentType, instanceArguments);
        if (suitableConstructor == null)
        {
            throw new NullReferenceException($"No suitable constructor found for {argument.Type}");
        }

        var instances = new List<object>();
        for (var i = argument.FlattenChildStartingIndex; i < argument.FlattenChildStartingIndex + argument.ArgumentCount; i++)
        {
            instances.Add(CreateInstanceRec(Arguments[i]));
        }
        
        var result = suitableConstructor.Invoke(instances.ToArray());
        return result;
    }

    private static ConstructorInfo? GetSuitableConstructor(Type argumentType, List<Type> instanceArguments)
    {
        // TODO clone instance arguments !
        ConstructorInfo? suitableConstructor = null;
        foreach (var constructor in argumentType.GetConstructors())
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length != instanceArguments.Count) continue;

            var validConstructor = true;
            foreach (var parameter in parameters)
            {
                var foundType = instanceArguments.Find(ia => ia.FullName == parameter.ParameterType.FullName);
                if (foundType == null)
                {
                    validConstructor = false;
                    break;  
                }

                instanceArguments.Remove(foundType);
            }

            if (!validConstructor) continue;
            suitableConstructor = constructor;
            
            break;
        }
        
        return suitableConstructor;
    }
    
    
}