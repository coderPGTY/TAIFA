namespace SharkSharp.Parser
{
    using System;

    using SharkSharp.Lexer;

    /// <summary>
    /// Исключение, возникающее при синтаксических ошибках или ошибках выполнения.
    /// </summary>
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
            this.Position = position;
        }

        /// <summary>
        /// Позиция в коде, где произошла ошибка.
        /// </summary>
        public SourcePosition Position { get; } = new SourcePosition(0, 0);
    }
}