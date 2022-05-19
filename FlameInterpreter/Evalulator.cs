using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlameInterpreter;
internal class Evalulator
{
    private readonly Runtime runtime;

    public Evalulator(Runtime runtime)
    {
        this.runtime = runtime;
    }

    public void EvalStatement(ref CodePointer pointer, out TypedObject? returned)
    {
        returned = null;

        switch (pointer.Current.Kind)
        {
            case TokenKind.Func:
                EvalFuncDefinition(ref pointer);
                return;
            case TokenKind.Return:
                pointer.Expect(TokenKind.Return);
                EvalExpression(ref pointer, out var result);
                returned = result;
                pointer.Expect(TokenKind.Semicolon);
                return;
            case TokenKind.If:
                returned = null;
                EvalIfStatement(ref pointer);
                return;
            case TokenKind.Identifier:
            case TokenKind.LeftParenthesis:
            case TokenKind.String:
            case TokenKind.Null:
            case TokenKind.Number:
                EvalExpression(ref pointer, out _);
                pointer.Expect(TokenKind.Semicolon);
                return;
            case TokenKind.Let:
                EvalVarDeclaration(ref pointer);
                return;
            case TokenKind.Semicolon:
                pointer.Expect(TokenKind.Semicolon);
                Console.WriteLine("warning: pointless semicolon");
                return;
            default:
                throw new Exception(pointer.Current.Kind.ToString());
        }
    }

    private void EvalIfStatement(ref CodePointer pointer)
    {
        pointer.Expect(TokenKind.If);
        EvalExpression(ref pointer, out TypedObject condition);

        if (condition.Type.Name != ObjectType.Int.Name)
        {
            throw new Exception($"Cannot convert from '{condition.Type.Name}' to int");
        }

        if (((int)condition.Object)>0)
        {
            runtime.BeginStackFrame();
            pointer.Expect(TokenKind.LeftBracket);
            EvalStatementBlock(ref pointer, out _);
            pointer.Expect(TokenKind.RightBracket);
            runtime.EndStackFrame();
        }
        else
        {
            SkipScope(ref pointer, 1);
        }
    }

    private void EvalVarDeclaration(ref CodePointer pointer)
    {
        var let = pointer.Expect(TokenKind.Let);
        var name = pointer.Expect(TokenKind.Identifier);
        var equals = pointer.Expect(TokenKind.Equals);

        EvalExpression(ref pointer, out var initialValue);
        
        runtime.RegisterVariable(initialValue.Type, name.ToString(), initialValue.Object);

        pointer.ExpectOrNull(TokenKind.Semicolon);
    }

    private void EvalFuncDefinition(ref CodePointer pointer)
    {
        pointer.Expect(TokenKind.Func);
        
        var name = pointer.Expect(TokenKind.Identifier);
        
        pointer.Expect(TokenKind.LeftParenthesis);
        
        var parameters = EvalFuncParameters(ref pointer);
        
        pointer.Expect(TokenKind.RightParenthesis);
        pointer.Expect(TokenKind.LeftBracket);
        
        var body = pointer;
        SkipScope(ref pointer, 1);
        pointer.Seek(-1);

        pointer.Expect(TokenKind.RightBracket);

        runtime.RegisterFunction(new FunctionDefinition(name.Value, parameters, body));
    }

    private void SkipScope(ref CodePointer pointer, int initialDepth)
    {
        int depth = initialDepth;
        while (depth > 0)
        {
            var t = pointer.MoveNext();

            if (t.Kind == TokenKind.LeftBracket)
                depth++;

            if (t.Kind == TokenKind.RightBracket)
                depth--;
        }
    }

    public void EvalStatementBlock(ref CodePointer pointer, out TypedObject? result)
    {
        result = null;

        while (pointer.Current.Kind != TokenKind.RightBracket && !pointer.IsAtEnd)
        {
            EvalStatement(ref pointer, out var r);

            if (r is not null)
            {
                result = r;
                return;
            }
        }
    }

    private FunctionParameter[] EvalFuncParameters(ref CodePointer pointer)
    {
        var parameters = new List<FunctionParameter>();
        while (pointer.Peek().Kind == TokenKind.Identifier)
        {
            var type = pointer.Expect(TokenKind.Identifier);
            var name = pointer.Expect(TokenKind.Identifier);

            parameters.Add(new(name.ToString(), new ObjectType(type.ToString())));

            var comma = pointer.ExpectOptional(TokenKind.Comma);

            if (comma is null)
                break;
        }

        return parameters.ToArray();
    }

    public void EvalExpression(ref CodePointer pointer, out TypedObject result, OperatorPrecedence precedence = OperatorPrecedence.Assignment)
    {
        switch (pointer.Current.Kind)
        {
            case TokenKind.LeftParenthesis:
                pointer.Expect(TokenKind.LeftParenthesis);
                EvalExpression(ref pointer, out result, (OperatorPrecedence)0);
                pointer.Expect(TokenKind.RightParenthesis);
                return;
            case TokenKind.RightParenthesis:
                result = new(ObjectType.Void, null);
                return;
            case TokenKind.Identifier:
            case TokenKind.Number:
                EvalOperatorExpression(ref pointer, out result, precedence);
                return;
            default:
                throw new Exception($"Unrecognized expression syntax of type {pointer.Current.Kind}");
        }
    }

    private void EvalOperatorExpression(ref CodePointer pointer, out TypedObject result, OperatorPrecedence precedence = OperatorPrecedence.Assignment)
    {
        if (pointer.Peek(0).Kind == TokenKind.Identifier)
        {
            if (pointer.Peek(1).Kind == TokenKind.LeftParenthesis)
            {
                EvalFuncCall(ref pointer, out result);
            }
            else
            {
                EvalVarReference(ref pointer, out result);
            }
        }
        else
        {
            result = new TypedObject(ObjectType.Int, int.Parse(pointer.Expect(TokenKind.Number).Value));
        }

        Token operand = pointer.Current;

        while (operand.IsOperator() && operand.GetPrecedence() >= precedence && !pointer.IsAtEnd)
        {
            operand = pointer.MoveNext();
            
            EvalExpression(ref pointer, out var right, operand.GetPrecedence());

            switch (operand.Value)
            {
                default:
                case "+":
                    result = new TypedObject(ObjectType.Int, (int) result.Object + (int) right.Object);
                    break;
                case "*":
                    result = new TypedObject(ObjectType.Int, (int) result.Object* (int) right.Object);
                    break;
                case "-":
                    result = new TypedObject(ObjectType.Int, (int) result.Object - (int) right.Object);
                    break;
                case "/":
                    result = new TypedObject(ObjectType.Int, (int) result.Object / (int) right.Object);
                    break;
            }

            operand = pointer.Current;
        }
    }

    private void EvalFuncCall(ref CodePointer pointer, out TypedObject result)
    {
        var id = pointer.Expect(TokenKind.Identifier);

        pointer.Expect(TokenKind.LeftParenthesis);
        EvalArgList(ref pointer, out var args);
        pointer.Expect(TokenKind.RightParenthesis);

        var func = runtime.GetVariable(id.ToString());

        if (func.value is Delegate del)
        {
            result = new TypedObject(del.DynamicInvoke(args.Select(a => a.Object).ToArray()));
        }
        else if (func.value is FunctionDefinition def)
        {
            runtime.BeginStackFrame();

            for (int i = 0; i < def.Parameters.Length; i++)
            {
                runtime.RegisterVariable(def.Parameters[i].Type, def.Parameters[i].Name, (int)args[i].Object);
            }

            var body = def.Body;
            EvalStatementBlock(ref body, out TypedObject? possibleResult);

            runtime.EndStackFrame();

            result = possibleResult ?? new TypedObject(null);
        }
        else
        {
            throw new Exception($"Cannot call non-function variable '{id}'");
        }

    }

    private void EvalArgList(ref CodePointer pointer, out TypedObject[] args)
    {
        List<TypedObject> result = new();

        while (pointer.Current.Kind != TokenKind.RightParenthesis)
        {
            EvalExpression(ref pointer, out var arg);
            result.Add(arg);

            if (pointer.ExpectOptional(TokenKind.Comma) is null)
            {
                break;
            }
        }

        args = result.ToArray();
    }

    private void EvalVarReference(ref CodePointer pointer, out TypedObject result)
    {
        var id = pointer.Expect(TokenKind.Identifier);
        var v = runtime.GetVariable(id.Value);
        result = new TypedObject(ObjectType.Int, v.value);
    }
}
