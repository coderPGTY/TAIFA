namespace SharkSharp.Lexer
{
    using System.Collections.Generic;

    /// <summary>
    /// Лексический анализатор языка SharkSharp. Читает исходный код посимвольно.
    /// </summary>
    public partial class Lexer
    {
        private readonly string source;
        private int position;
        private int line;
        private int column;

        /// <summary>
        /// Инициализирует новый экземпляр лексера.
        /// </summary>
        public Lexer(string source)
        {
            this.source = source;
            this.position = 0;
            this.line = 1;
            this.column = 1;
        }

        /// <summary>
        /// Разбирает весь исходный код и возвращает список токенов.
        /// </summary>
        public List<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();
            while (true)
            {
                Token token = this.GetNextToken();
                tokens.Add(token);
                if (token.Type == TokenType.Eof)
                {
                    break;
                }
            }

            return tokens;
        }

        private Token GetNextToken()
        {
            this.SkipWhitespaceAndComments();
            if (this.IsAtEnd())
            {
                return new Token(TokenType.Eof, string.Empty, this.line, this.column);
            }

            int startLine = this.line;
            int startColumn = this.column;
            char currentChar = this.Peek();

            if (currentChar == '"')
            {
                return this.ReadString(startLine, startColumn);
            }

            if (IsAsciiDigit(currentChar))
            {
                return this.ReadNumber(startLine, startColumn);
            }

            if (IsAsciiLetter(currentChar) || currentChar == '_')
            {
                return this.ReadIdentifierOrKeyword(startLine, startColumn);
            }

            return this.ReadOperatorOrDelimiter(startLine, startColumn);
        }

        private void SkipWhitespaceAndComments()
        {
            while (!this.IsAtEnd())
            {
                char c = this.Peek();
                if (c == ' ' || c == '\t' || c == '\n' || c == '\r')
                {
                    this.Advance();
                }
                else if (c == '/' && this.PeekNext() == '/')
                {
                    while (!this.IsAtEnd() && this.Peek() != '\n')
                    {
                        this.Advance();
                    }
                }
                else if (c == '/' && this.PeekNext() == '*')
                {
                    this.Advance();
                    this.Advance();
                    while (!this.IsAtEnd())
                    {
                        if (this.Peek() == '*' && this.PeekNext() == '/')
                        {
                            this.Advance();
                            this.Advance();
                            break;
                        }

                        this.Advance();
                    }

                    if (this.IsAtEnd())
                    {
                        throw new LexerException("Незакрытый блочный комментарий", this.CurrentPosition());
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
            if (this.IsAtEnd())
            {
                return '\0';
            }

            return this.source[this.position];
        }

        private char PeekNext()
        {
            if (this.position + 1 >= this.source.Length)
            {
                return '\0';
            }

            return this.source[this.position + 1];
        }

        private void Advance()
        {
            if (!this.IsAtEnd())
            {
                if (this.source[this.position] == '\n')
                {
                    this.line++;
                    this.column = 1;
                }
                else
                {
                    this.column++;
                }

                this.position++;
            }
        }

        private bool IsAtEnd()
        {
            return this.position >= this.source.Length;
        }

        private SourcePosition CurrentPosition()
        {
            return new SourcePosition(this.line, this.column);
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
}