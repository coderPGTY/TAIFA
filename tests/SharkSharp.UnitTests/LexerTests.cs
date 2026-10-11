using System;
using System.Collections.Generic;

using SharkSharp.Lexer;
using Xunit;

namespace SharkSharp.UnitTests;

public class LexerTests
{
    public static TheoryData<string, List<TokenType>> ValidTokenData => new TheoryData<string, List<TokenType>>
    {
        { "let x: int = 5;", new List<TokenType> { TokenType.Let, TokenType.Identifier, TokenType.Colon, TokenType.Int, TokenType.Assign, TokenType.IntegerLiteral, TokenType.Semicolon, TokenType.Eof } },
        { "main", new List<TokenType> { TokenType.Identifier, TokenType.Eof } },
        { "true false", new List<TokenType> { TokenType.True, TokenType.False, TokenType.Eof } },
        { "_privateVar123", new List<TokenType> { TokenType.Identifier, TokenType.Eof } },
        { "0", new List<TokenType> { TokenType.IntegerLiteral, TokenType.Eof } },
        { "42", new List<TokenType> { TokenType.IntegerLiteral, TokenType.Eof } },
        { "\"hello\"", new List<TokenType> { TokenType.StringLiteral, TokenType.Eof } },
        { "\"shark\\n\"", new List<TokenType> { TokenType.StringLiteral, TokenType.Eof } },
        { "\"escape \\\"quote\\\"\"", new List<TokenType> { TokenType.StringLiteral, TokenType.Eof } },
        { "\"tab\\there\"", new List<TokenType> { TokenType.StringLiteral, TokenType.Eof } },
        { "+ - * / %", new List<TokenType> { TokenType.Plus, TokenType.Minus, TokenType.Star, TokenType.Slash, TokenType.Percent, TokenType.Eof } },
        { "== != < <= > >=", new List<TokenType> { TokenType.Equal, TokenType.NotEqual, TokenType.Less, TokenType.LessEqual, TokenType.Greater, TokenType.GreaterEqual, TokenType.Eof } },
        { "&& || !", new List<TokenType> { TokenType.And, TokenType.Or, TokenType.Not, TokenType.Eof } },
        { "= . [ ] ( ) { } , ; :", new List<TokenType> { TokenType.Assign, TokenType.Dot, TokenType.LeftBracket, TokenType.RightBracket, TokenType.LeftParen, TokenType.RightParen, TokenType.LeftBrace, TokenType.RightBrace, TokenType.Comma, TokenType.Semicolon, TokenType.Colon, TokenType.Eof } },
        { "let x = 5; // это комментарий", new List<TokenType> { TokenType.Let, TokenType.Identifier, TokenType.Assign, TokenType.IntegerLiteral, TokenType.Semicolon, TokenType.Eof } },
        { "/* блочный комментарий */ let y = 10;", new List<TokenType> { TokenType.Let, TokenType.Identifier, TokenType.Assign, TokenType.IntegerLiteral, TokenType.Semicolon, TokenType.Eof } },
        { " \t \r \n let \n a = 1;", new List<TokenType> { TokenType.Let, TokenType.Identifier, TokenType.Assign, TokenType.IntegerLiteral, TokenType.Semicolon, TokenType.Eof } },
    };

    public static TheoryData<string, string> ErrorTokenData => new TheoryData<string, string>
    {
        { "007", "ведущего нуля" },
        { "\"unclosed string", "Незакрытая строковая константа" },
        { "\"invalid escape \\q\"", "Недопустимая escape-последовательность" },
        { "/* unclosed block", "Незакрытый блочный комментарий" },
        { "@invalid", "Неизвестный символ" },
        { "a & b", "Ожидалось '&&'" },
        { "a | b", "Ожидалось '||'" },
    };

    [Theory]
    [MemberData(nameof(ValidTokenData))]
    public void Tokenize_ValidInput_ReturnsExpectedTokens(string source, List<TokenType> expectedTypes)
    {
        SharkSharp.Lexer.Lexer lexer = new SharkSharp.Lexer.Lexer(source);
        List<Token> tokens = lexer.Tokenize();

        Assert.Equal(expectedTypes.Count, tokens.Count);
        for (int i = 0; i < expectedTypes.Count; i++)
        {
            Assert.Equal(expectedTypes[i], tokens[i].Type);
        }
    }

    [Theory]
    [MemberData(nameof(ErrorTokenData))]
    public void Tokenize_InvalidInput_ThrowsLexerException(string source, string expectedMessagePart)
    {
        SharkSharp.Lexer.Lexer lexer = new SharkSharp.Lexer.Lexer(source);
        Action testAction = () => lexer.Tokenize();
        LexerException exception = Assert.Throws<LexerException>(testAction);
        Assert.Contains(expectedMessagePart, exception.Message);
    }
}