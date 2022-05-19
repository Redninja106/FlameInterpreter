using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameInterpreter;
public sealed class ObjectType
{
    public static readonly ObjectType Bool = new("bool");
    public static readonly ObjectType Int = new("int");
    public static readonly ObjectType Function = new("function");
    public static readonly ObjectType Extern = new("extern");
    public static readonly ObjectType Void = new("void");

    public string Name { get; private set; }

    public ObjectType(string name)
    {
        Name = name;
    }
    internal static ObjectType GetForObject(object obj)
    {
        if (obj is null)
        {
            return Void;
        }
        if (obj is bool)
        {
            return Bool;
        }
        else if (obj is int)
        {
            return Int;
        }
        else if (obj is FunctionDefinition)
        {
            return Function;
        }
        else if (obj is Delegate)
        {
            return Extern;
        }
        else
        {
            throw new Exception("Unknown object type");
        }
        
    }
}
