namespace SharkSharp.Parser
{
    using System.Collections.Generic;

    /// <summary>
    /// Тестовый дублёр (Fake) окружения для приемочных и модульных тестов.
    /// </summary>
    public class FakeEnvironment : IEnvironment
    {
        private readonly Queue<string> inputs;

        public FakeEnvironment(IEnumerable<string> inputs)
        {
            this.inputs = new Queue<string>(inputs);
        }

        public List<long> Outputs { get; } = new List<long>();

        public void WriteLine(long value)
        {
            this.Outputs.Add(value);
        }

        public string? ReadLine()
        {
            return this.inputs.Count > 0 ? this.inputs.Dequeue() : string.Empty;
        }
    }
}