namespace SharkSharp.Parser;

/// <summary>
/// Реализация окружения для консольного приложения.
/// </summary>
public class ConsoleEnvironment : IEnvironment
{
    /// <summary>
    /// Выводит целое число в поток вывода.
    /// </summary>
    public void WriteLine(int value)
    {
        System.Console.WriteLine(value);
    }

    /// <summary>
    /// Читает строку из потока ввода.
    /// </summary>
    public string? ReadLine()
    {
        return System.Console.ReadLine();
    }
}