namespace SharkSharp.Parser
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using SharkSharp.Lexer;

    /// <summary>
    /// Исключение, возникающее при синтаксических ошибках или ошибках выполнения.
    /// </summary>
    [SuppressMessage(
        "Roslynator",
        "RCS1194:Implement exception constructors",
        Justification = "Внутреннее исключение проекта. Стандартные конструкторы не используются.")]
    public sealed class ParseException : Exception
    {
        /// <summary>
        /// Инициализирует новое исключение с сообщением и позицией в исходном коде.
        /// </summary>
        public ParseException(string message, SourcePosition position)
            : base($"{message} (строка {position.Line}, столбец {position.Column})")
        {
            this.Position = position;
        }

        /// <summary>
        /// Позиция в коде, где произошла ошибка.
        /// </summary>
        public SourcePosition Position { get; }
    }
}