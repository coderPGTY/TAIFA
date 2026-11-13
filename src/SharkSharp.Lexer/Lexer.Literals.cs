namespace SharkSharp.Lexer
{
    using System;

    /// <summary>
    /// Часть лексера для разбора числовых и строковых литералов.
    /// </summary>
    public partial class Lexer
    {
        private Token ReadNumber(int startLine, int startColumn)
        {
            int startPos = this.position;
            while (!this.IsAtEnd() && IsAsciiDigit(this.Peek()))
            {
                this.Advance();
            }

            string lexeme = this.source.Substring(startPos, this.position - startPos);

            if (lexeme.Length > 1 && lexeme[0] == '0')
            {
                throw new LexerException("Целочисленный литерал не может начинаться с ведущего нуля", new SourcePosition(startLine, startColumn));
            }

            char nextChar = this.Peek();
            if (IsAsciiLetter(nextChar) || nextChar == '_')
            {
                throw new LexerException($"Недопустимый символ '{nextChar}' сразу после числового литерала", new SourcePosition(startLine, startColumn));
            }

            if (!long.TryParse(lexeme, out long _))
            {
                throw new LexerException($"Целочисленный литерал выходит за допустимый диапазон 64-битного знакового целого: {lexeme}", new SourcePosition(startLine, startColumn));
            }

            return new Token(TokenType.IntegerLiteral, lexeme, startLine, startColumn);
        }

        private Token ReadString(int startLine, int startColumn)
        {
            int startPos = this.position;
            this.Advance();

            while (!this.IsAtEnd() && this.Peek() != '"')
            {
                char c = this.Peek();
                if (c == '\n' || c == '\r')
                {
                    throw new LexerException("Незакрытая строковая константа: запрещён перенос строки внутри литерала", this.CurrentPosition());
                }

                if (c == '\\')
                {
                    this.Advance();
                    if (this.IsAtEnd())
                    {
                        throw new LexerException("Незавершенная escape-последовательность в строке", this.CurrentPosition());
                    }

                    char escaped = this.Peek();
                    this.Advance();

                    switch (escaped)
                    {
                        case 'n':
                        case 't':
                        case '"':
                        case '\\':
                            break;
                        default:
                            throw new LexerException($"Недопустимая escape-последовательность '\\{escaped}'", this.CurrentPosition());
                    }
                }
                else
                {
                    this.Advance();
                }
            }

            if (this.IsAtEnd())
            {
                throw new LexerException("Незакрытая строковая константа", this.CurrentPosition());
            }

            this.Advance();
            string rawLexeme = this.source.Substring(startPos, this.position - startPos);
            return new Token(TokenType.StringLiteral, rawLexeme, startLine, startColumn);
        }
    }
}