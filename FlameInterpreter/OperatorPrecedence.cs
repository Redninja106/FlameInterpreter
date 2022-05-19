using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameInterpreter;

public enum OperatorPrecedence
{
    Assignment,
    Conditional,
    Coalescing,
    ConditionalOr,
    ConditionalAnd,
    LogicalOr,
    LogicalXor,
    LogicalAnd,
    Equality,
    Shift,
    Additive,
    Multiplicative,
    Unary,
    Cast
}
