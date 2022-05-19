using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameInterpreter;
internal struct CodePointer
{
    private int position = 0;
    private readonly Document document;

    public Document Document => document;
    public int Position
    {
        get => position;
        set => position = value >= 0 && value < document.tokens.Length ? value : throw new Exception("Reached end of file!");
    }
    
    public ref Token Current => ref document.tokens[position];
    public bool IsAtEnd => position >= document.tokens.Length - 1;

    public CodePointer(Document document)
    {
        this.document = document;
    }

    public Token? ExpectOptional(TokenKind kind)
    {
        return Current.Kind == kind ? MoveNext() : null;
    }

    public Token? ExpectOrNull(TokenKind kind, out Token foundToken)
    {
        foundToken = MoveNext();
        return foundToken.Kind == kind ? foundToken : null;
    }

    public Token? ExpectOrNull(TokenKind kind)
    {
        return ExpectOrNull(kind, out _);
    }

    public Token Expect(TokenKind kind)
    {
        return ExpectOrNull(kind, out var t) ?? throw new Exception($"Expected {kind} but got {t.Kind}");
    }

    /// <summary>
    /// Moves the document's position forward by one token and returns the previous token.
    /// </summary>
    public Token MoveNext()
    {
        Seek(1);
        return Peek(-1);
    }

    /// <summary>
    /// returns the token at the specified offset relative to the current position.
    /// </summary>
    public Token Peek(int offset = 1)
    {
        return document.tokens[Position + offset];
    }

    /// <summary>
    /// Moves the document's position by the specified amount and returns the token at the old position.
    /// </summary>
    public Token Seek(int offset)
    {
        return document.tokens[Position += offset];
    }
}
