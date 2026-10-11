namespace SharkSharp.Lexer;

public partial class Lexer
{
    private Token ReadIdentifierOrKeyword(int startLine, int startColumn)
    {
        int startPos = _position;

        while (!IsAtEnd() && (IsAsciiLetterOrDigit(Peek()) || Peek() == '_'))
        {
            Advance();
        }

        string text = _source.Substring(startPos, _position - startPos);

        if (Keywords.TryGetKeyword(text, out TokenType keywordType))
        {
            return new Token(keywordType, text, startLine, startColumn);
        }

        return new Token(TokenType.Identifier, text, startLine, startColumn);
    }
}