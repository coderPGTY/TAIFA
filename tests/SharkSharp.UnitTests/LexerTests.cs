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
        { "a", new List<TokenType> { TokenType.Identifier, TokenType.Eof } },
        { "Sum sum", new List<TokenType> { TokenType.Identifier, TokenType.Identifier, TokenType.Eof } },
        { "string bool struct func return if else while", new List<TokenType> { TokenType.String, TokenType.Bool, TokenType.Struct, TokenType.Func, TokenType.Return, TokenType.If, TokenType.Else, TokenType.While, TokenType.Eof } },
        { "0", new List<TokenType> { TokenType.IntegerLiteral, TokenType.Eof } },
        { "42", new List<TokenType> { TokenType.IntegerLiteral, TokenType.Eof } },
        { "9223372036854775807", new List<TokenType> { TokenType.IntegerLiteral, TokenType.Eof } },
        { "\"hello\"", new List<TokenType> { TokenType.StringLiteral, TokenType.Eof } },
        { "\"shark\\n\"", new List<TokenType> { TokenType.StringLiteral, TokenType.Eof } },
        { "\"escape \\\"quote\\\"\"", new List<TokenType> { TokenType.StringLiteral, TokenType.Eof } },
        { "\"tab\\there\"", new List<TokenType> { TokenType.StringLiteral, TokenType.Eof } },
        { "\"backslash\\\\\"", new List<TokenType> { TokenType.StringLiteral, TokenType.Eof } },
        { "\"empty\"", new List<TokenType> { TokenType.StringLiteral, TokenType.Eof } },
        { "+ - * / %", new List<TokenType> { TokenType.Plus, TokenType.Minus, TokenType.Star, TokenType.Slash, TokenType.Percent, TokenType.Eof } },
        { "== != < <= > >=", new List<TokenType> { TokenType.Equal, TokenType.NotEqual, TokenType.Less, TokenType.LessEqual, TokenType.Greater, TokenType.GreaterEqual, TokenType.Eof } },
        { "&& || !", new List<TokenType> { TokenType.And, TokenType.Or, TokenType.Not, TokenType.Eof } },
        { "= . [ ] ( ) { } , ; :", new List<TokenType> { TokenType.Assign, TokenType.Dot, TokenType.LeftBracket, TokenType.RightBracket, TokenType.LeftParen, TokenType.RightParen, TokenType.LeftBrace, TokenType.RightBrace, TokenType.Comma, TokenType.Semicolon, TokenType.Colon, TokenType.Eof } },
        { "let x = 5; // это комментарий", new List<TokenType> { TokenType.Let, TokenType.Identifier, TokenType.Assign, TokenType.IntegerLiteral, TokenType.Semicolon, TokenType.Eof } },
        { "/* блочный комментарий */ let y = 10;", new List<TokenType> { TokenType.Let, TokenType.Identifier, TokenType.Assign, TokenType.IntegerLiteral, TokenType.Semicolon, TokenType.Eof } },
        { "/**/\nlet z = 1;", new List<TokenType> { TokenType.Let, TokenType.Identifier, TokenType.Assign, TokenType.IntegerLiteral, TokenType.Semicolon, TokenType.Eof } },
        { "/*\nмногострочный\nкомментарий\n*/ x", new List<TokenType> { TokenType.Identifier, TokenType.Eof } },
        { " \t \r \n let \n a = 1;", new List<TokenType> { TokenType.Let, TokenType.Identifier, TokenType.Assign, TokenType.IntegerLiteral, TokenType.Semicolon, TokenType.Eof } },
        { "   x   ", new List<TokenType> { TokenType.Identifier, TokenType.Eof } },
        { "let\r\nx=1;", new List<TokenType> { TokenType.Let, TokenType.Identifier, TokenType.Assign, TokenType.IntegerLiteral, TokenType.Semicolon, TokenType.Eof } },
        { "", new List<TokenType> { TokenType.Eof } },
        { "   ", new List<TokenType> { TokenType.Eof } },
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
        { "\"line1\nline2\"", "перенос строки" },
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

    [Fact]
    public void Tokenize_MultilineInput_TracksLineAndColumn()
    {
        string source = "let\nx";
        SharkSharp.Lexer.Lexer lexer = new SharkSharp.Lexer.Lexer(source);
        List<Token> tokens = lexer.Tokenize();

        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(1, tokens[0].Line);
        Assert.Equal(1, tokens[0].Column);

        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal(2, tokens[1].Line);
        Assert.Equal(1, tokens[1].Column);
    }

    [Fact]
    public void Tokenize_CRLFInput_TracksLineAndColumn()
    {
        string source = "let\r\nx";
        SharkSharp.Lexer.Lexer lexer = new SharkSharp.Lexer.Lexer(source);
        List<Token> tokens = lexer.Tokenize();

        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(1, tokens[0].Line);
        Assert.Equal(1, tokens[0].Column);

        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal(2, tokens[1].Line);
        Assert.Equal(1, tokens[1].Column);
    }

    [Fact]
    public void Tokenize_SingleLineInput_TracksColumn()
    {
        string source = "let x";
        SharkSharp.Lexer.Lexer lexer = new SharkSharp.Lexer.Lexer(source);
        List<Token> tokens = lexer.Tokenize();

        Assert.Equal(TokenType.Let, tokens[0].Type);
        Assert.Equal(1, tokens[0].Column);

        Assert.Equal(TokenType.Identifier, tokens[1].Type);
        Assert.Equal(5, tokens[1].Column);
    }

    [Fact]
    public void Tokenize_EofToken_HasCorrectPosition()
    {
        string source = "x";
        SharkSharp.Lexer.Lexer lexer = new SharkSharp.Lexer.Lexer(source);
        List<Token> tokens = lexer.Tokenize();

        Token eof = tokens[tokens.Count - 1];
        Assert.Equal(TokenType.Eof, eof.Type);
        Assert.Equal(1, eof.Line);
        Assert.Equal(2, eof.Column);
    }

    [Fact]
    public void LexerException_DefaultConstructor_CreatesInstance()
    {
        LexerException exception = new LexerException();
        Assert.NotNull(exception);
        Assert.Equal(0, exception.Position.Line);
        Assert.Equal(0, exception.Position.Column);
    }

    [Fact]
    public void LexerException_MessageConstructor_SetsMessage()
    {
        LexerException exception = new LexerException("test error");
        Assert.Contains("test error", exception.Message);
    }

    [Fact]
    public void LexerException_InnerExceptionConstructor_SetsInnerException()
    {
        Exception inner = new InvalidOperationException("inner");
        LexerException exception = new LexerException("outer", inner);
        Assert.Contains("outer", exception.Message);
        Assert.Same(inner, exception.InnerException);
    }

    [Fact]
    public void LexerException_PositionConstructor_FormatsMessage()
    {
        SourcePosition position = new SourcePosition(5, 10);
        LexerException exception = new LexerException("error", position);
        Assert.Contains("error", exception.Message);
        Assert.Contains("5", exception.Message);
        Assert.Contains("10", exception.Message);
        Assert.Equal(5, exception.Position.Line);
        Assert.Equal(10, exception.Position.Column);
    }
}