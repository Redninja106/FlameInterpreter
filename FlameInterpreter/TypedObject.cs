using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameInterpreter;
public struct TypedObject
{
    public TypedObject(object @object)
    {
        Object = @object;
        Type = ObjectType.GetForObject(@object);
    }

    
    public TypedObject(ObjectType type, object @object)
    {
        Type = type;
        Object = @object;
    }

    public ObjectType Type { get; private set; }
    public object Object { get; private set; }
}
