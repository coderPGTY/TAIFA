namespace SharkSharp.Parser
{
    using System;
    using System.Collections.Generic;

    using SharkSharp.Lexer;

    /// <summary>
    /// Парсер и интерпретатор языка SharkSharp, использующий метод рекурсивного спуска.
    /// Вычисляет значения выражений непосредственно в процессе разбора.
    /// </summary>
    public class Parser
    {
        private readonly List<Token> tokens;
        private int position;
        private readonly IEnvironment env;
        private long lastValue;

        public Parser(List<Token> tokens, IEnvironment env)
        {
            this.tokens = tokens;
            this.position = 0;
            this.env = env;
        }

        /// <summary>
        /// Запускает разбор и интерпретацию всей программы.
        /// Соответствует правилу EBNF: program
        /// </summary>
        public long ParseProgram()
        {
            this.ParseMainFunction();
            this.Match(TokenType.Eof);
            return this.lastValue;
        }

        /// <summary>
        /// Разбирает функцию main.
        /// Соответствует правилу EBNF: main_function
        /// </summary>
        private void ParseMainFunction()
        {
            this.Match(TokenType.Func);

            Token name = this.Match(TokenType.Identifier);
            if (name.Lexeme != "main")
            {
                throw new ParseException("Точкой входа должна быть функция 'main'", new SourcePosition(name.Line, name.Column));
            }

            this.Match(TokenType.LeftParen);
            this.Match(TokenType.RightParen);
            this.ParseBlock();
        }

        private void ParseBlock()
        {
            this.Match(TokenType.LeftBrace);

            while (!this.IsAtEnd() && this.Peek().Type != TokenType.RightBrace)
            {
                this.ParseStatement();
            }

            this.Match(TokenType.RightBrace);
        }

        private void ParseStatement()
        {
            if (this.Peek().Type == TokenType.Identifier && this.Peek().Lexeme == "print")
            {
                this.ParsePrintCall();
            }
            else
            {
                this.lastValue = this.ParseExpression();
            }

            this.Match(TokenType.Semicolon);
        }

        private void ParsePrintCall()
        {
            this.Match(TokenType.Identifier);
            this.Match(TokenType.LeftParen);

            long value = this.ParseExpression();

            this.Match(TokenType.RightParen);

            this.env.WriteLine(value);
            this.lastValue = value;
        }

        private long ParseExpression()
        {
            return this.ParseAdditiveExpression();
        }

        private long ParseAdditiveExpression()
        {
            long left = this.ParseMultiplicativeExpression();

            while (this.Peek().Type == TokenType.Plus || this.Peek().Type == TokenType.Minus)
            {
                Token op = this.Advance();
                long right = this.ParseMultiplicativeExpression();
                left = CheckedArithmetic(left, right, op.Type, op);
            }

            return left;
        }

        private long ParseMultiplicativeExpression()
        {
            long left = this.ParseUnaryExpression();

            while (this.Peek().Type == TokenType.Star || this.Peek().Type == TokenType.Slash || this.Peek().Type == TokenType.Percent)
            {
                Token op = this.Advance();
                long right = this.ParseUnaryExpression();

                if ((op.Type == TokenType.Slash || op.Type == TokenType.Percent) && right == 0)
                {
                    throw new ParseException(
                        op.Type == TokenType.Slash ? "Деление на ноль" : "Остаток от деления на ноль",
                        new SourcePosition(op.Line, op.Column));
                }

                left = CheckedArithmetic(left, right, op.Type, op);
            }

            return left;
        }

        private long ParseUnaryExpression()
        {
            if (this.Peek().Type == TokenType.Minus)
            {
                Token op = this.Advance();
                long value = this.ParseUnaryExpression();
                try
                {
                    return checked(-value);
                }
                catch (OverflowException)
                {
                    throw new ParseException(
                        "Результат арифметической операции вне диапазона типа int",
                        new SourcePosition(op.Line, op.Column));
                }
            }

            return this.ParsePrimaryExpression();
        }

        private long ParsePrimaryExpression()
        {
            Token current = this.Peek();

            if (current.Type == TokenType.IntegerLiteral)
            {
                this.Advance();
                return long.Parse(current.Lexeme);
            }

            if (current.Type == TokenType.LeftParen)
            {
                this.Advance();
                long value = this.ParseExpression();
                this.Match(TokenType.RightParen);
                return value;
            }

            if (current.Type == TokenType.Identifier)
            {
                return this.ParseFunctionCall();
            }

            throw new ParseException($"Ожидалось число, скобка или функция, но встречено '{current.Lexeme}'", new SourcePosition(current.Line, current.Column));
        }

        /// <summary>
        /// Разбирает вызов функции.
        /// Соответствует правилу EBNF: function_call
        /// </summary>
        private long ParseFunctionCall()
        {
            Token name = this.Advance();
            this.Match(TokenType.LeftParen);

            List<long> arguments = new List<long>();
            if (this.Peek().Type != TokenType.RightParen)
            {
                arguments = this.ParseArgumentList();
            }

            this.Match(TokenType.RightParen);

            return this.EvaluateBuiltinFunction(name.Lexeme, arguments, name);
        }

        /// <summary>
        /// Разбирает список аргументов функции.
        /// Соответствует правилу EBNF: argument_list
        /// </summary>
        private List<long> ParseArgumentList()
        {
            List<long> arguments = new List<long>();
            arguments.Add(this.ParseExpression());

            while (this.Peek().Type == TokenType.Comma)
            {
                this.Advance();
                arguments.Add(this.ParseExpression());
            }

            return arguments;
        }

        private long EvaluateBuiltinFunction(string functionName, List<long> arguments, Token token)
        {
            switch (functionName)
            {
                case "readInt":
                    if (arguments.Count != 0)
                    {
                        throw new ParseException("Функция readInt не принимает аргументов", new SourcePosition(token.Line, token.Column));
                    }

                    string input = this.env.ReadLine() ?? string.Empty;
                    if (!long.TryParse(input, out long parsedValue))
                    {
                        throw new ParseException("Ошибка ввода: ожидалось целое число", new SourcePosition(token.Line, token.Column));
                    }

                    return parsedValue;

                case "abs":
                    if (arguments.Count != 1)
                    {
                        throw new ParseException("Функция abs принимает ровно 1 аргумент", new SourcePosition(token.Line, token.Column));
                    }

                    return Math.Abs(arguments[0]);

                case "min":
                    if (arguments.Count != 2)
                    {
                        throw new ParseException("Функция min принимает ровно 2 аргумента", new SourcePosition(token.Line, token.Column));
                    }

                    return Math.Min(arguments[0], arguments[1]);

                case "max":
                    if (arguments.Count != 2)
                    {
                        throw new ParseException("Функция max принимает ровно 2 аргумента", new SourcePosition(token.Line, token.Column));
                    }

                    return Math.Max(arguments[0], arguments[1]);

                default:
                    throw new ParseException($"Неизвестная функция '{functionName}'", new SourcePosition(token.Line, token.Column));
            }
        }

        /// <summary>
        /// Выполняет арифметическую операцию с контролем переполнения long.
        /// При переполнении превращает OverflowException в ParseException на русском.
        /// </summary>
        private static long CheckedArithmetic(long left, long right, TokenType operatorType, Token operatorToken)
        {
            try
            {
                return operatorType switch
                {
                    TokenType.Plus => checked(left + right),
                    TokenType.Minus => checked(left - right),
                    TokenType.Star => checked(left * right),
                    TokenType.Slash => checked(left / right),
                    TokenType.Percent => checked(left % right),
                    _ => throw new ParseException(
                        $"Неизвестный оператор '{operatorToken.Lexeme}'",
                        new SourcePosition(operatorToken.Line, operatorToken.Column)),
                };
            }
            catch (OverflowException)
            {
                throw new ParseException(
                    "Результат арифметической операции вне диапазона типа int",
                    new SourcePosition(operatorToken.Line, operatorToken.Column));
            }
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