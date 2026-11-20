namespace SharkSharp.Cli
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using SharkSharp.Ast;
    using SharkSharp.Evaluator;
    using SharkSharp.Lexer;
    using SharkSharp.Parser;

    /// <summary>
    /// Точка входа консольного приложения SharkSharp.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Главный метод приложения.
        /// </summary>
        public static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Использование: SharkSharp.Cli <путь_к_файлу>");
                return 1;
            }

            string filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.Error.WriteLine($"Ошибка: файл '{filePath}' не найден.");
                return 1;
            }

            try
            {
                string sourceCode = File.ReadAllText(filePath);

                Lexer lexer = new Lexer(sourceCode);
                List<Token> tokens = lexer.Tokenize();

                Parser parser = new Parser(tokens);
                ProgramNode ast = parser.ParseProgram();

                IEnvironment env = new ConsoleEnvironment();
                AstEvaluator evaluator = new AstEvaluator(env);
                evaluator.Evaluate(ast);

                return 0;
            }
            catch (LexerException ex)
            {
                Console.Error.WriteLine($"Лексическая ошибка: {ex.Message}");
                return 1;
            }
            catch (ParseException ex)
            {
                Console.Error.WriteLine($"Ошибка выполнения: {ex.Message}");
                return 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Непредвиденная ошибка: {ex.Message}");
                return 1;
            }
        }
    }
}