using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameInterpreter;

public class Interpreter
{
    private readonly Lexer lexer = new();

    public void Run(string code)
    {
        lexer.Lex(code, out var tokens);
        var document = new Document(tokens);
        
        Console.ForegroundColor = ConsoleColor.Gray;
        var rt = new Runtime(rt => rt.RegisterExtern("print", new Action<object>(Console.WriteLine)), document);

        try
        {
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
        }
    }
}
