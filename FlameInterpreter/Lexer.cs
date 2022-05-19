using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlameInterpreter;
public class Lexer
{
    public void Lex(string input, out Token[] tokens)
    {
        var tokenList = new List<Token>();

        var current = "";
        for (int i = 0; i < input.Length; i++)
        {
            current += input[i];
            
            if (i == input.Length - 1)
                continue;

            var multi = (current + input[i + 1]).Trim() switch
            {
                "->" or "::" => current + input[i + 1],
                var word when Regex.IsMatch(word, @"^[a-zA-Z][a-zA-Z0-9]*$") => current + input[i + 1],
                var str when Regex.IsMatch(str, @"^""[^""]*""?$") => current + input[i + 1],
                var number when Regex.IsMatch(number, @"^((\d+\.\d*)|(\d*\.\d+)|(\d+))$") => current + input[i + 1],
                _ => current
            };
            
            if (current == multi)
            {
                if (!string.IsNullOrWhiteSpace(current.Trim()))
                    tokenList.Add(new Token(current.Trim()));
                
                current = "";
            }
        }
        
        if (!string.IsNullOrWhiteSpace(current.Trim()))
            tokenList.Add(new Token(current.Trim()));

        tokenList.Add(new Token("") { Kind = TokenKind.EndOfFile } );

        tokens = tokenList.ToArray();
    }
}