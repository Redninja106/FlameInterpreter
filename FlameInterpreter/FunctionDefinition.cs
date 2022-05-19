using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameInterpreter;

internal record FunctionDefinition(string Name, FunctionParameter[] Parameters, CodePointer Body)
{
}