namespace SharkSharp.Parser;

/// <summary>
/// Интерфейс для абстрагирования операций ввода-вывода интерпретатора.
/// </summary>
public interface IEnvironment
{
    /// <summary>
    /// Выводит целое число в поток вывода.
    /// </summary>
    void WriteLine(long value);

    /// <summary>
    /// Читает строку из потока ввода.
    /// </summary>
    string? ReadLine();
}