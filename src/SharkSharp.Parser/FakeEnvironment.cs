namespace SharkSharp.Parser
{
    using System.Collections.Generic;

    /// <summary>
    /// Тестовый дублёр (Fake) окружения для приемочных тестов.
    /// </summary>
    public class FakeEnvironment : IEnvironment
    {
        private readonly Queue<string> inputs;

        /// <summary>
        /// Инициализирует новый экземпляр тестового окружения.
        /// </summary>
        public FakeEnvironment(IEnumerable<string> inputs)
        {
            this.inputs = new Queue<string>(inputs);
        }

        /// <summary>
        /// Список выведенных значений для проверки в тестах.
        /// </summary>
        public List<long> Outputs { get; } = new List<long>();

        /// <summary>
        /// Выводит целое число в поток вывода.
        /// </summary>
        public void WriteLine(long value)
        {
            this.Outputs.Add(value);
        }

        /// <summary>
        /// Читает строку из потока ввода.
        /// </summary>
        public string? ReadLine()
        {
            return this.inputs.Count > 0 ? this.inputs.Dequeue() : string.Empty;
        }
    }
}