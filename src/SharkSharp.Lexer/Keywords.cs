using System.Collections.Generic;

namespace SharkSharp.Lexer;

/// <summary>
/// Вспомогательный класс для быстрого поиска ключевых слов.
/// </summary>
internal static class Keywords
{
    private static readonly Dictionary<string, TokenType> Map = new()
    {
        { "int", TokenType.Int },
        { "string", TokenType.String },
        { "bool", TokenType.Bool },
        { "struct", TokenType.Struct },
        { "let", TokenType.Let },
        { "func", TokenType.Func },
        { "return", TokenType.Return },
        { "if", TokenType.If },
        { "else", TokenType.Else },
        { "while", TokenType.While },
        { "true", TokenType.True },
        { "false", TokenType.False },
    };

    /// <summary>
    /// Пытается получить тип токена для переданного текста.
    /// </summary>
    public static bool TryGetKeyword(string text, out TokenType type)
    {
        return Map.TryGetValue(text, out type);
    }
}