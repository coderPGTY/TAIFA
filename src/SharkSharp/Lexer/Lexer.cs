using System.Collections.Generic;

namespace SharkSharp.Lexer;

/// <summary>
/// Лексический анализатор языка SharkSharp. Читает исходный код посимвольно.
/// </summary>
public partial class Lexer
{
    private readonly string _source;
    private int _position;
    private int _line;
    private int _column;

    public Lexer(string source)
    {
        _source = source;
        _position = 0;
        _line = 1;
        _column = 1;
    }

    /// <summary>
    /// Разбирает весь исходный код и возвращает список токенов.
    /// </summary>
    public List<Token> Tokenize()
    {
        List<Token> tokens = new List<Token>();

        while (true)
        {
            Token token = GetNextToken();
            tokens.Add(token);

            if (token.Type == TokenType.Eof)
            {
                break;
            }
        }

        return tokens;
    }

    /// <summary>
    /// Считывает и возвращает следующий токен из потока.
    /// </summary>
    private Token GetNextToken()
    {
        SkipWhitespaceAndComments();

        if (IsAtEnd())
        {
            return new Token(TokenType.Eof, "", _line, _column);
        }

        int startLine = _line;
        int startColumn = _column;
        char currentChar = Peek();

        if (currentChar == '"')
        {
            return ReadString(startLine, startColumn);
        }

        if (IsAsciiDigit(currentChar))
        {
            return ReadNumber(startLine, startColumn);
        }

        if (IsAsciiLetter(currentChar) || currentChar == '_')
        {
            return ReadIdentifierOrKeyword(startLine, startColumn);
        }

        return ReadOperatorOrDelimiter(startLine, startColumn);
    }

    private void SkipWhitespaceAndComments()
    {
        while (!IsAtEnd())
        {
            char c = Peek();

            // Явная проверка только тех символов, что указаны в спецификации
            if (c == ' ' || c == '\t' || c == '\n' || c == '\r')
            {
                Advance();
            }
            else if (c == '/' && PeekNext() == '/')
            {
                while (!IsAtEnd() && Peek() != '\n')
                {
                    Advance();
                }
            }
            else if (c == '/' && PeekNext() == '*')
            {
                Advance();
                Advance();

                while (!IsAtEnd())
                {
                    if (Peek() == '*' && PeekNext() == '/')
                    {
                        Advance();
                        Advance();
                        break;
                    }

                    Advance();
                }

                if (IsAtEnd())
                {
                    throw new LexerException("Незакрытый блочный комментарий", CurrentPosition());
                }
            }
            else
            {
                break;
            }
        }
    }

    private char Peek()
    {
        if (IsAtEnd())
        {
            return '\0';
        }

        return _source[_position];
    }

    private char PeekNext()
    {
        if (_position + 1 >= _source.Length)
        {
            return '\0';
        }

        return _source[_position + 1];
    }

    private void Advance()
    {
        if (!IsAtEnd())
        {
            if (_source[_position] == '\n')
            {
                _line++;
                _column = 1;
            }
            else
            {
                _column++;
            }

            _position++;
        }
    }

    private bool IsAtEnd()
    {
        return _position >= _source.Length;
    }

    private SourcePosition CurrentPosition()
    {
        return new SourcePosition(_line, _column);
    }

    private static bool IsAsciiLetter(char c)
    {
        return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
    }

    private static bool IsAsciiDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private static bool IsAsciiLetterOrDigit(char c)
    {
        return IsAsciiLetter(c) || IsAsciiDigit(c);
    }
}