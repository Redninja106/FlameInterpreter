using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameInterpreter;
internal class Document
{
    public readonly Token[] tokens;

    public Document(Token[] tokens)
    {
        this.tokens = tokens;
    }
}
