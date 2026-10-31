using System;
using System.Collections.Generic;

using SharkSharp.Lexer;

namespace SharkSharp.Parser;

/// <summary>
/// Парсер и интерпретатор языка SharkSharp, использующий метод рекурсивного спуска.
/// Вычисляет значения выражений непосредственно в процессе разбора.
/// </summary>
public class Parser
{
    private readonly List<Token> _tokens;
    private int _position;
    private readonly IEnvironment _env;

    public Parser(List<Token> tokens, IEnvironment env)
    {
        _tokens = tokens;
        _position = 0;
        _env = env;
    }

    /// <summary>
    /// Запускает разбор и интерпретацию всей программы.
    /// </summary>
    public void ParseProgram()
    {
        ParseMainFunction();
        Match(TokenType.Eof);
    }

    private void ParseMainFunction()
    {
        Match(TokenType.Func);

        Token name = Match(TokenType.Identifier);
        if (name.Lexeme != "main")
        {
            throw new ParseException("Точкой входа должна быть функция 'main'", new SourcePosition(name.Line, name.Column));
        }

        Match(TokenType.LeftParen);
        Match(TokenType.RightParen);
        ParseBlock();
    }

    private void ParseBlock()
    {
        Match(TokenType.LeftBrace);

        while (!IsAtEnd() && Peek().Type != TokenType.RightBrace)
        {
            ParseStatement();
        }

        Match(TokenType.RightBrace);
    }

    private void ParseStatement()
    {
        if (Peek().Type == TokenType.Identifier && Peek().Lexeme == "print")
        {
            ParsePrintCall();
        }
        else
        {
            ParseExpression();
        }

        Match(TokenType.Semicolon);
    }

    private void ParsePrintCall()
    {
        Match(TokenType.Identifier);
        Match(TokenType.LeftParen);

        int value = ParseExpression();

        Match(TokenType.RightParen);

        _env.WriteLine(value);
    }

    private int ParseExpression()
    {
        return ParseAdditiveExpression();
    }

    private int ParseAdditiveExpression()
    {
        int left = ParseMultiplicativeExpression();

        while (Peek().Type == TokenType.Plus || Peek().Type == TokenType.Minus)
        {
            Token op = Advance();
            int right = ParseMultiplicativeExpression();

            if (op.Type == TokenType.Plus)
            {
                left = left + right;
            }
            else
            {
                left = left - right;
            }
        }

        return left;
    }

    private int ParseMultiplicativeExpression()
    {
        int left = ParseUnaryExpression();

        while (Peek().Type == TokenType.Star || Peek().Type == TokenType.Slash || Peek().Type == TokenType.Percent)
        {
            Token op = Advance();
            int right = ParseUnaryExpression();

            if (op.Type == TokenType.Star)
            {
                left = left * right;
            }
            else if (op.Type == TokenType.Slash)
            {
                if (right == 0)
                {
                    throw new ParseException("Деление на ноль", new SourcePosition(op.Line, op.Column));
                }

                left = left / right;
            }
            else if (op.Type == TokenType.Percent)
            {
                if (right == 0)
                {
                    throw new ParseException("Остаток от деления на ноль", new SourcePosition(op.Line, op.Column));
                }

                left = left % right;
            }
        }

        return left;
    }

    private int ParseUnaryExpression()
    {
        if (Peek().Type == TokenType.Minus)
        {
            Token op = Advance();
            int value = ParseUnaryExpression();
            return -value;
        }

        return ParsePrimaryExpression();
    }

    private int ParsePrimaryExpression()
    {
        Token current = Peek();

        if (current.Type == TokenType.IntegerLiteral)
        {
            Advance();
            return int.Parse(current.Lexeme);
        }

        if (current.Type == TokenType.LeftParen)
        {
            Advance();
            int value = ParseExpression();
            Match(TokenType.RightParen);
            return value;
        }

        if (current.Type == TokenType.Identifier)
        {
            return ParseFunctionCall();
        }

        throw new ParseException($"Ожидалось число, скобка или функция, но встречено '{current.Lexeme}'", new SourcePosition(current.Line, current.Column));
    }

    private int ParseFunctionCall()
    {
        Token name = Advance();
        Match(TokenType.LeftParen);

        List<int> arguments = new List<int>();
        if (Peek().Type != TokenType.RightParen)
        {
            arguments.Add(ParseExpression());
            while (Peek().Type == TokenType.Comma)
            {
                Advance();
                arguments.Add(ParseExpression());
            }
        }

        Match(TokenType.RightParen);

        return EvaluateBuiltinFunction(name.Lexeme, arguments, name);
    }

    private int EvaluateBuiltinFunction(string functionName, List<int> arguments, Token token)
    {
        switch (functionName)
        {
            case "readInt":
                if (arguments.Count != 0)
                {
                    throw new ParseException("Функция readInt не принимает аргументов", new SourcePosition(token.Line, token.Column));
                }

                string input = _env.ReadLine() ?? string.Empty;
                return int.Parse(input);

            case "abs":
                if (arguments.Count != 1)
                {
                    throw new ParseException("Функция abs принимает ровно 1 аргумент", new SourcePosition(token.Line, token.Column));
                }

                return Math.Abs(arguments[0]);

            case "min":
                if (arguments.Count != 2)
                {
                    throw new ParseException("Функция min принимает ровно 2 аргумента", new SourcePosition(token.Line, token.Column));
                }

                return Math.Min(arguments[0], arguments[1]);

            case "max":
                if (arguments.Count != 2)
                {
                    throw new ParseException("Функция max принимает ровно 2 аргумента", new SourcePosition(token.Line, token.Column));
                }

                return Math.Max(arguments[0], arguments[1]);

            default:
                throw new ParseException($"Неизвестная функция '{functionName}'", new SourcePosition(token.Line, token.Column));
        }
    }

    private Token Peek()
    {
        if (IsAtEnd())
        {
            return _tokens[_tokens.Count - 1];
        }

        return _tokens[_position];
    }

    private Token Advance()
    {
        if (!IsAtEnd())
        {
            _position++;
        }

        return _tokens[_position - 1];
    }

    private Token Match(TokenType type)
    {
        if (Peek().Type == type)
        {
            return Advance();
        }

        throw new ParseException($"Ожидался токен {type}, но встречен {Peek().Type}", new SourcePosition(Peek().Line, Peek().Column));
    }

    private bool IsAtEnd()
    {
        return _position >= _tokens.Count || _tokens[_position].Type == TokenType.Eof;
    }
}