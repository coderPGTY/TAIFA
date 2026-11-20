namespace SharkSharp.Evaluator
{
    using System;
    using System.Collections.Generic;

    using SharkSharp.Ast;
    using SharkSharp.Lexer;
    using SharkSharp.Parser;

    /// <summary>
    /// Интерпретатор AST, выполняющий вычисления стековым методом.
    /// Делегирует ввод-вывод через интерфейс IEnvironment.
    /// </summary>
    public class AstEvaluator : IAstVisitor
    {
        private readonly Stack<long> stack;
        private readonly IEnvironment env;

        public AstEvaluator(IEnvironment env)
        {
            this.stack = new Stack<long>();
            this.env = env;
        }

        /// <summary>
        /// Запускает выполнение программы и возвращает результат последнего выражения.
        /// </summary>
        public long Evaluate(ProgramNode program)
        {
            program.Accept(this);
            return this.stack.Count > 0 ? this.stack.Pop() : 0;
        }

        public void VisitIntegerLiteral(IntegerLiteralExpression node)
        {
            this.stack.Push(node.Value);
        }

        public void VisitBinary(BinaryExpression node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            long right = this.stack.Pop();
            long left = this.stack.Pop();
            this.stack.Push(CheckedArithmetic(left, right, node.Operator));
        }

        public void VisitUnary(UnaryExpression node)
        {
            node.Operand.Accept(this);
            long value = this.stack.Pop();
            try
            {
                this.stack.Push(checked(-value));
            }
            catch (OverflowException)
            {
                throw new ParseException("Результат арифметической операции вне диапазона типа int", new SourcePosition(0, 0));
            }
        }

        public void VisitFunctionCall(FunctionCallExpression node)
        {
            List<long> arguments = new List<long>();
            foreach (Expression arg in node.Arguments)
            {
                arg.Accept(this);
                arguments.Add(this.stack.Pop());
            }

            arguments.Reverse();

            long result = this.EvaluateBuiltin(node.Name, arguments);
            this.stack.Push(result);
        }

        public void VisitExpressionStatement(ExpressionStatement node)
        {
            node.Expression.Accept(this);
        }

        public void VisitPrintStatement(PrintStatement node)
        {
            node.Expression.Accept(this);
            long value = this.stack.Pop();
            this.env.WriteLine(value);
            this.stack.Push(value);
        }

        public void VisitBlock(BlockStatement node)
        {
            foreach (Statement statement in node.Statements)
            {
                statement.Accept(this);
            }
        }

        public void VisitMainFunction(MainFunctionDeclaration node)
        {
            node.Body.Accept(this);
        }

        public void VisitProgram(ProgramNode node)
        {
            node.MainFunction.Accept(this);
        }

        private long EvaluateBuiltin(string functionName, List<long> arguments)
        {
            switch (functionName)
            {
                case "readInt":
                    if (arguments.Count != 0)
                    {
                        throw new ParseException("Функция readInt не принимает аргументов", new SourcePosition(0, 0));
                    }

                    string input = this.env.ReadLine() ?? string.Empty;
                    if (!long.TryParse(input, out long parsedValue))
                    {
                        throw new ParseException("Ошибка ввода: ожидалось целое число", new SourcePosition(0, 0));
                    }

                    return parsedValue;
                case "abs":
                    if (arguments.Count != 1)
                    {
                        throw new ParseException("Функция abs принимает ровно 1 аргумент", new SourcePosition(0, 0));
                    }

                    return Math.Abs(arguments[0]);
                case "min":
                    if (arguments.Count != 2)
                    {
                        throw new ParseException("Функция min принимает ровно 2 аргумента", new SourcePosition(0, 0));
                    }

                    return Math.Min(arguments[0], arguments[1]);
                case "max":
                    if (arguments.Count != 2)
                    {
                        throw new ParseException("Функция max принимает ровно 2 аргумента", new SourcePosition(0, 0));
                    }

                    return Math.Max(arguments[0], arguments[1]);
                default:
                    throw new ParseException($"Неизвестная функция '{functionName}'", new SourcePosition(0, 0));
            }
        }

        private static long CheckedArithmetic(long left, long right, TokenType operatorType)
        {
            try
            {
                if ((operatorType == TokenType.Slash || operatorType == TokenType.Percent) && right == 0)
                {
                    throw new DivideByZeroException();
                }

                return operatorType switch
                {
                    TokenType.Plus => checked(left + right),
                    TokenType.Minus => checked(left - right),
                    TokenType.Star => checked(left * right),
                    TokenType.Slash => checked(left / right),
                    TokenType.Percent => checked(left % right),
                    _ => throw new ParseException("Неизвестный оператор", new SourcePosition(0, 0)),
                };
            }
            catch (OverflowException)
            {
                throw new ParseException("Результат арифметической операции вне диапазона типа int", new SourcePosition(0, 0));
            }
            catch (DivideByZeroException)
            {
                throw new ParseException("Деление на ноль", new SourcePosition(0, 0));
            }
        }
    }
}