namespace SharkSharp.Lexer
{
    /// <summary>
    /// Часть лексера для разбора идентификаторов и ключевых слов.
    /// </summary>
    public partial class Lexer
    {
        private Token ReadIdentifierOrKeyword(int startLine, int startColumn)
        {
            int startPos = this.position;
            while (!this.IsAtEnd() && (IsAsciiLetterOrDigit(this.Peek()) || this.Peek() == '_'))
            {
                this.Advance();
            }

            string text = this.source.Substring(startPos, this.position - startPos);
            if (Keywords.TryGetKeyword(text, out TokenType keywordType))
            {
                return new Token(keywordType, text, startLine, startColumn);
            }

            return new Token(TokenType.Identifier, text, startLine, startColumn);
        }
    }
}