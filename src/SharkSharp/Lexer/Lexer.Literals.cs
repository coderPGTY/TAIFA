namespace SharkSharp.Lexer;

public partial class Lexer
{
    private Token ReadNumber(int startLine, int startColumn)
    {
        int startPos = _position;

        while (!IsAtEnd() && IsAsciiDigit(Peek()))
        {
            Advance();
        }

        string lexeme = _source.Substring(startPos, _position - startPos);

        if (lexeme.Length > 1 && lexeme.StartsWith("0"))
        {
            throw new LexerException("Целочисленный литерал не может начинаться с ведущего нуля", new SourcePosition(startLine, startColumn));
        }

        return new Token(TokenType.IntegerLiteral, lexeme, startLine, startColumn);
    }

    private Token ReadString(int startLine, int startColumn)
    {
        int startPos = _position;
        Advance();

        while (!IsAtEnd() && Peek() != '"')
        {
            char c = Peek();

            if (c == '\n' || c == '\r')
            {
                throw new LexerException("Незакрытая строковая константа: запрещён перенос строки внутри литерала", CurrentPosition());
            }

            if (c == '\\')
            {
                Advance();

                if (IsAtEnd())
                {
                    throw new LexerException("Незавершенная escape-последовательность в строке", CurrentPosition());
                }

                char escaped = Peek();
                Advance();

                switch (escaped)
                {
                    case 'n':
                    case 't':
                    case '"':
                    case '\\':
                        break;
                    default:
                        throw new LexerException($"Недопустимая escape-последовательность '\\{escaped}'", CurrentPosition());
                }
            }
            else
            {
                Advance();
            }
        }

        if (IsAtEnd())
        {
            throw new LexerException("Незакрытая строковая константа", CurrentPosition());
        }

        Advance();
        string rawLexeme = _source.Substring(startPos, _position - startPos);
        return new Token(TokenType.StringLiteral, rawLexeme, startLine, startColumn);
    }
}