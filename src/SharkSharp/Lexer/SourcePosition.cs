namespace SharkSharp.Lexer;

/// <summary>
/// Представляет позицию в исходном коде (строка и столбец).
/// </summary>
public record SourcePosition(int Line, int Column);