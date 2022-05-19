using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameInterpreter;

public enum TokenKind
{
    None,
    Func,
    Let,
    Return,
    If,
    Semicolon,
    Colon,
    ColonColon,
    Equals,
    Comma,
    LeftParenthesis,
    RightParenthesis,
    LeftBracket,
    RightBracket,
    Pointer,
    Identifier,
    String,
    Number,
    Null,
    EndOfFile,
    True,
    False,
}