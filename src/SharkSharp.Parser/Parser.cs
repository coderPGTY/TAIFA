namespace SharkSharp.Parser
{
    using System.Collections.Generic;

    using SharkSharp.Ast;
    using SharkSharp.Lexer;

    /// <summary>
    /// Парсер языка SharkSharp, строящий абстрактное синтаксическое дерево (AST).
    /// Использует метод рекурсивного спуска.
    /// </summary>
    public class Parser
    {
        private readonly List<Token> tokens;
        private int position;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
            this.position = 0;
        }

        /// <summary>
        /// Разбирает всю программу и возвращает корневой узел AST.
        /// </summary>
        public ProgramNode ParseProgram()
        {
            MainFunctionDeclaration mainFunc = this.ParseMainFunction();
            this.Match(TokenType.Eof);
            return new ProgramNode(mainFunc);
        }

        private MainFunctionDeclaration ParseMainFunction()
        {
            this.Match(TokenType.Func);
            Token name = this.Match(TokenType.Identifier);
            if (name.Lexeme != "main")
            {
                throw new ParseException("Точкой входа должна быть функция 'main'", new SourcePosition(name.Line, name.Column));
            }

            this.Match(TokenType.LeftParen);
            this.Match(TokenType.RightParen);
            BlockStatement body = this.ParseBlock();
            return new MainFunctionDeclaration(body);
        }

        private BlockStatement ParseBlock()
        {
            this.Match(TokenType.LeftBrace);
            List<Statement> statements = new List<Statement>();
            while (!this.IsAtEnd() && this.Peek().Type != TokenType.RightBrace)
            {
                statements.Add(this.ParseStatement());
            }

            this.Match(TokenType.RightBrace);
            return new BlockStatement(statements);
        }

        private Statement ParseStatement()
        {
            if (this.Peek().Type == TokenType.Identifier && this.Peek().Lexeme == "print")
            {
                return this.ParsePrintCall();
            }

            Expression expr = this.ParseExpression();
            this.Match(TokenType.Semicolon);
            return new ExpressionStatement(expr);
        }

        private PrintStatement ParsePrintCall()
        {
            this.Match(TokenType.Identifier);
            this.Match(TokenType.LeftParen);
            Expression expr = this.ParseExpression();
            this.Match(TokenType.RightParen);
            this.Match(TokenType.Semicolon);
            return new PrintStatement(expr);
        }

        private Expression ParseExpression()
        {
            return this.ParseAdditiveExpression();
        }

        private Expression ParseAdditiveExpression()
        {
            Expression left = this.ParseMultiplicativeExpression();
            while (this.Peek().Type == TokenType.Plus || this.Peek().Type == TokenType.Minus)
            {
                Token op = this.Advance();
                Expression right = this.ParseMultiplicativeExpression();
                left = new BinaryExpression(left, op.Type, right);
            }

            return left;
        }

        private Expression ParseMultiplicativeExpression()
        {
            Expression left = this.ParseUnaryExpression();
            while (this.Peek().Type == TokenType.Star || this.Peek().Type == TokenType.Slash || this.Peek().Type == TokenType.Percent)
            {
                Token op = this.Advance();
                Expression right = this.ParseUnaryExpression();
                left = new BinaryExpression(left, op.Type, right);
            }

            return left;
        }

        private Expression ParseUnaryExpression()
        {
            if (this.Peek().Type == TokenType.Minus)
            {
                Token op = this.Advance();
                Expression operand = this.ParseUnaryExpression();
                return new UnaryExpression(op.Type, operand);
            }

            return this.ParsePrimaryExpression();
        }

        private Expression ParsePrimaryExpression()
        {
            Token current = this.Peek();
            if (current.Type == TokenType.IntegerLiteral)
            {
                this.Advance();
                return new IntegerLiteralExpression(long.Parse(current.Lexeme));
            }

            if (current.Type == TokenType.LeftParen)
            {
                this.Advance();
                Expression expr = this.ParseExpression();
                this.Match(TokenType.RightParen);
                return expr;
            }

            if (current.Type == TokenType.Identifier)
            {
                return this.ParseFunctionCall();
            }

            throw new ParseException($"Ожидалось число, скобка или функция, но встречено '{current.Lexeme}'", new SourcePosition(current.Line, current.Column));
        }

        private FunctionCallExpression ParseFunctionCall()
        {
            Token name = this.Advance();
            this.Match(TokenType.LeftParen);
            List<Expression> arguments = new List<Expression>();
            if (this.Peek().Type != TokenType.RightParen)
            {
                arguments = this.ParseArgumentList();
            }

            this.Match(TokenType.RightParen);
            return new FunctionCallExpression(name.Lexeme, arguments);
        }

        private List<Expression> ParseArgumentList()
        {
            List<Expression> arguments = new List<Expression>();
            arguments.Add(this.ParseExpression());
            while (this.Peek().Type == TokenType.Comma)
            {
                this.Advance();
                arguments.Add(this.ParseExpression());
            }

            return arguments;
        }

        private Token Peek()
        {
            if (this.IsAtEnd())
            {
                return this.tokens[this.tokens.Count - 1];
            }

            return this.tokens[this.position];
        }

        private Token Advance()
        {
            if (!this.IsAtEnd())
            {
                this.position++;
            }

            return this.tokens[this.position - 1];
        }

        private Token Match(TokenType type)
        {
            if (this.Peek().Type == type)
            {
                return this.Advance();
            }

            throw new ParseException($"Ожидался токен {type}, но встречен {this.Peek().Type}", new SourcePosition(this.Peek().Line, this.Peek().Column));
        }

        private bool IsAtEnd()
        {
            return this.position >= this.tokens.Count || this.tokens[this.position].Type == TokenType.Eof;
        }
    }
}