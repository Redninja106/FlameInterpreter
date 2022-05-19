using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameInterpreter;

internal class Runtime
{
    public Evalulator Evalulator { get; }

    List<Document> documents = new();
    
    Stack<List<VariableDefinition>> variables = new();

    public Runtime(Action<Runtime> initialize, params Document[] documents)
    {
        BeginStackFrame();

        Evalulator = new(this);

        initialize(this);
        
        Array.ForEach(documents, RegisterDocument);
    }

    public void RegisterExtern(string name, Delegate func)
    {
        RegisterVariable(ObjectType.Extern, name, func);
        Console.WriteLine($"[runtime: New Extern Defined - {name}]");
    }

    public void RegisterFunction(FunctionDefinition definition)
    {
        RegisterVariable(ObjectType.Function, definition.Name, definition);
        Console.WriteLine($"[runtime: New Function Defined - {definition.Name} ( {definition.Parameters.Aggregate("", (a, b) => a += $"{b.Type.Name} {b.Name} ")}) ]");
    }

    public VariableDefinition GetVariable(string name)
    {
        return variables.First(v => v.Any(u => u.Name == name)).First(v => v.Name == name);
    }

    public void BeginStackFrame()
    {
        variables.Push(new());
    }

    public void EndStackFrame()
    {
        if (variables.Count <= 1)
            throw new Exception();
        
        variables.Pop();
    }

    public void RegisterVariable(ObjectType type, string name, object initialValue = null)
    {
        foreach (var frame in variables)
        {
            foreach (var v in frame)
            {
                if (v.Name == name)
                    throw new Exception();
            }
        }

        variables.Peek().Add(new VariableDefinition(name, type, initialValue));

        // Console.WriteLine($"[runtime: New Variable of type {type.Name} Defined - {name}]");
    }

    private bool MatchFuncParameters(FunctionDefinition func, IEnumerable<ObjectType> parameters)
    {
        if (func.Parameters.Length != parameters.Count())
            return false;
        
        for (int i = 0; i < func.Parameters.Length; i++)
        {
            if (func.Parameters[i].Type.Name != parameters.ElementAt(i).Name)
                return false;
        }

        return true;
    }

    public void RegisterDocument(Document document)
    {
        documents.Add(document);

        var pointer = new CodePointer(document);

        Evalulator.EvalStatementBlock(ref pointer, out _);
    }
}
