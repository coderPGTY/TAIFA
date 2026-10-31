using System;
using System.IO;

using SharkSharp.Lexer;
using SharkSharp.Parser;

namespace SharkSharp.Cli;

public static class Program
{
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

            SharkSharp.Lexer.Lexer lexer = new SharkSharp.Lexer.Lexer(sourceCode);
            List<Token> tokens = lexer.Tokenize();

            // ДОБАВЛЕН ConsoleEnvironment
            IEnvironment env = new ConsoleEnvironment();
            SharkSharp.Parser.Parser parser = new SharkSharp.Parser.Parser(tokens, env);
            parser.ParseProgram();

            return 0;
        }
        catch (LexerException ex)
        {
            Console.Error.WriteLine($"Лексическая ошибка: {ex.Message}");
            return 1;
        }
        catch (ParseException ex)
        {
            Console.Error.WriteLine($"Синтаксическая ошибка или ошибка выполнения: {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Непредвиденная ошибка: {ex.Message}");
            return 1;
        }
    }
}