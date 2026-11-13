using System;

using SharkSharp.Lexer;

namespace SharkSharp.Parser;

public sealed class ParseException : Exception
{
    public ParseException()
    {
    }

    public ParseException(string message)
        : base(message)
    {
    }

    public ParseException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public ParseException(string message, SourcePosition position)
        : base($"{message} (строка {position.Line}, столбец {position.Column})")
    {
        Position = position;
    }

    public SourcePosition Position { get; } = new SourcePosition(0, 0);
}