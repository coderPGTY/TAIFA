namespace SharkSharp.Lexer;

/// <summary>
/// Все типы токенов языка SharkSharp.
/// </summary>
public enum TokenType
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    Identifier,

    /// <summary>
    /// Целочисленный литерал.
    /// </summary>
    IntegerLiteral,

    /// <summary>
    /// Строковый литерал.
    /// </summary>
    StringLiteral,

    /// <summary>
    /// Ключевое слово int.
    /// </summary>
    Int,

    /// <summary>
    /// Ключевое слово string.
    /// </summary>
    String,

    /// <summary>
    /// Ключевое слово bool.
    /// </summary>
    Bool,

    /// <summary>
    /// Ключевое слово struct.
    /// </summary>
    Struct,

    /// <summary>
    /// Ключевое слово let.
    /// </summary>
    Let,

    /// <summary>
    /// Ключевое слово func.
    /// </summary>
    Func,

    /// <summary>
    /// Ключевое слово return.
    /// </summary>
    Return,

    /// <summary>
    /// Ключевое слово if.
    /// </summary>
    If,

    /// <summary>
    /// Ключевое слово else.
    /// </summary>
    Else,

    /// <summary>
    /// Ключевое слово while.
    /// </summary>
    While,

    /// <summary>
    /// Ключевое слово true.
    /// </summary>
    True,

    /// <summary>
    /// Ключевое слово false.
    /// </summary>
    False,

    /// <summary>
    /// Оператор сложения (+).
    /// </summary>
    Plus,

    /// <summary>
    /// Оператор вычитания (-).
    /// </summary>
    Minus,

    /// <summary>
    /// Оператор умножения (*).
    /// </summary>
    Star,

    /// <summary>
    /// Оператор деления (/).
    /// </summary>
    Slash,

    /// <summary>
    /// Оператор остатка от деления (%).
    /// </summary>
    Percent,

    /// <summary>
    /// Оператор равенства (==).
    /// </summary>
    Equal,

    /// <summary>
    /// Оператор неравенства (!=).
    /// </summary>
    NotEqual,

    /// <summary>
    /// Оператор меньше (<).
    /// </summary>
    Less,

    /// <summary>
    /// Оператор больше (>).
    /// </summary>
    Greater,

    /// <summary>
    /// Оператор меньше или равно (<=).
    /// </summary>
    LessEqual,

    /// <summary>
    /// Оператор больше или равно (>=).
    /// </summary>
    GreaterEqual,

    /// <summary>
    /// Логическое И (&&).
    /// </summary>
    And,

    /// <summary>
    /// Логическое ИЛИ (||).
    /// </summary>
    Or,

    /// <summary>
    /// Логическое НЕ (!).
    /// </summary>
    Not,

    /// <summary>
    /// Оператор присваивания (=).
    /// </summary>
    Assign,

    /// <summary>
    /// Левая фигурная скобка ({).
    /// </summary>
    LeftBrace,

    /// <summary>
    /// Правая фигурная скобка (}).
    /// </summary>
    RightBrace,

    /// <summary>
    /// Левая круглая скобка (( ).
    /// </summary>
    LeftParen,

    /// <summary>
    /// Правая круглая скобка () ).
    /// </summary>
    RightParen,

    /// <summary>
    /// Левая квадратная скобка ([ ).
    /// </summary>
    LeftBracket,

    /// <summary>
    /// Правая квадратная скобка (] ).
    /// </summary>
    RightBracket,

    /// <summary>
    /// Точка с запятой (;).
    /// </summary>
    Semicolon,

    /// <summary>
    /// Запятая (,).
    /// </summary>
    Comma,

    /// <summary>
    /// Точка (.).
    /// </summary>
    Dot,

    /// <summary>
    /// Двоеточие (:).
    /// </summary>
    Colon,

    /// <summary>
    /// Конец файла (Eof).
    /// </summary>
    Eof,
}