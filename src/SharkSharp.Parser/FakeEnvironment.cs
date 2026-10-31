using System.Collections.Generic;

namespace SharkSharp.Parser;

/// <summary>
/// Тестовый дублёр (Fake) окружения для приемочных тестов.
/// </summary>
public class FakeEnvironment : IEnvironment
{
    private readonly Queue<string> _inputs;

    /// <summary>
    /// Инициализирует новый экземпляр тестового окружения.
    /// </summary>
    public FakeEnvironment(IEnumerable<string> inputs)
    {
        _inputs = new Queue<string>(inputs);
    }

    /// <summary>
    /// Список выведенных значений для проверки в тестах.
    /// </summary>
    public List<int> Outputs { get; } = new List<int>();

    /// <summary>
    /// Выводит целое число в поток вывода.
    /// </summary>
    public void WriteLine(int value)
    {
        Outputs.Add(value);
    }

    /// <summary>
    /// Читает строку из потока ввода.
    /// </summary>
    public string? ReadLine()
    {
        return _inputs.Count > 0 ? _inputs.Dequeue() : string.Empty;
    }
}