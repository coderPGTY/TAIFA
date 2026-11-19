namespace SharkSharp.Lexer
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Исключение, возникающее при ошибках лексического анализа.
    /// </summary>
    [SuppressMessage(
        "Roslynator",
        "RCS1194:Implement exception constructors",
        Justification = "Внутреннее исключение проекта. Стандартные конструкторы не используются.")]
    public sealed class LexerException : Exception
    {
        /// <summary>
        /// Инициализирует новое исключение с сообщением и позицией в исходном коде.
        /// </summary>
        public LexerException(string message, SourcePosition position)
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