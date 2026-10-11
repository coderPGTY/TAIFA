namespace SharkSharp.Lexer;

/// <summary>
/// Неизменяемая структура (record), представляющая единичный токен.
/// </summary>
public record Token(TokenType Type, string Lexeme, int Line, int Column);