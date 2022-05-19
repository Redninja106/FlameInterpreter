using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlameInterpreter;
public struct Token
{
    public string Value;
    public TokenKind Kind;
    public TokenCategory Category;

    public Token(string value)
    {
        Value = value;
        (Kind, Category) = GetKindAndCategory(value);
    }

    private static (TokenKind kind, TokenCategory category) GetKindAndCategory(string value)
    {
        return value switch
        {
            "func" => (TokenKind.Func, TokenCategory.Keyword),
            "if" => (TokenKind.If, TokenCategory.Keyword),
            "return" => (TokenKind.Return, TokenCategory.Keyword),
            "true" => (TokenKind.True, TokenCategory.Keyword),
            "false" => (TokenKind.False, TokenCategory.Keyword),
            ";" => (TokenKind.Semicolon, TokenCategory.Symbol),
            ":" => (TokenKind.Colon, TokenCategory.Symbol),
            "::" => (TokenKind.ColonColon, TokenCategory.Symbol),
            "(" => (TokenKind.LeftParenthesis, TokenCategory.Symbol),
            ")" => (TokenKind.RightParenthesis, TokenCategory.Symbol),
            "{" => (TokenKind.LeftBracket, TokenCategory.Symbol),
            "}" => (TokenKind.RightBracket, TokenCategory.Symbol),
            "->" => (TokenKind.Pointer, TokenCategory.Symbol),
            "," => (TokenKind.Comma, TokenCategory.Symbol),
            "=" => (TokenKind.Equals, TokenCategory.Symbol),
            "let" => (TokenKind.Let, TokenCategory.Keyword),
            var str when Regex.IsMatch(str, @"^""[^""]*""?$") => (TokenKind.String, TokenCategory.Literal),
            var word when Regex.IsMatch(word, @"^[a-zA-Z][a-zA-Z0-9]*$") => (TokenKind.Identifier, TokenCategory.Identifier),
            var number when Regex.IsMatch(number, @"^((\d+\.\d*)|(\d*\.\d+)|(\d+))$") => (TokenKind.Number, TokenCategory.Literal),
            _ => (TokenKind.None, TokenCategory.None)
        };
    }

    public OperatorPrecedence GetPrecedence()
    {
        return Value switch
        {
            "+" => OperatorPrecedence.Additive,
            "-" => OperatorPrecedence.Additive,
            "*" => OperatorPrecedence.Multiplicative,
            "/" => OperatorPrecedence.Multiplicative,
            _ => (OperatorPrecedence)int.MaxValue,
        };
    }

    public bool IsOperator()
    {
        return GetPrecedence() != (OperatorPrecedence)int.MaxValue;
    }

    public enum TokenCategory
    {
        None,
        Keyword,
        Symbol,
        //Operator,
        //Type,
        Identifier,
        Literal,
        Util
    }

    public override string ToString()
    {
        return Value;
    }
}