using System;

namespace SharkSharp.Lexer;

/// <summary>
/// Исключение, возникающее при ошибках лексического анализа.
/// </summary>
public sealed class LexerException : Exception
{
    public LexerException()
    {
    }

    public LexerException(string message)
        : base(message)
    {
    }

    public LexerException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public LexerException(string message, SourcePosition position)
        : base($"{message} (строка {position.Line}, столбец {position.Column})")
    {
        Position = position;
    }

    /// <summary>
    /// Позиция в коде, где произошла ошибка.
    /// </summary>
    public SourcePosition Position { get; } = new SourcePosition(0, 0);
}