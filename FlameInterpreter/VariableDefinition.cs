using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameInterpreter;

internal record VariableDefinition(string Name, ObjectType type, object value)
{
}