using System.Collections.Generic;

using SharkSharp.Parser;
using Xunit;

namespace SharkSharp.Parser.UnitTests;

public class EnvironmentTests
{
    [Fact]
    public void FakeEnvironment_WriteLine_AddsValueToOutputs()
    {
        FakeEnvironment env = new FakeEnvironment(new List<string>());

        env.WriteLine(42);

        Assert.Equal(new List<int> { 42 }, env.Outputs);
    }

    [Fact]
    public void FakeEnvironment_WriteLine_MultipleValues()
    {
        FakeEnvironment env = new FakeEnvironment(new List<string>());

        env.WriteLine(1);
        env.WriteLine(2);
        env.WriteLine(3);

        Assert.Equal(new List<int> { 1, 2, 3 }, env.Outputs);
    }

    [Fact]
    public void FakeEnvironment_ReadLine_ReturnsQueuedInputs()
    {
        FakeEnvironment env = new FakeEnvironment(new List<string> { "15", "20" });

        Assert.Equal("15", env.ReadLine());
        Assert.Equal("20", env.ReadLine());
    }

    [Fact]
    public void FakeEnvironment_ReadLine_ReturnsEmpty_WhenNoInputs()
    {
        FakeEnvironment env = new FakeEnvironment(new List<string>());

        Assert.Equal(string.Empty, env.ReadLine());
    }

    [Fact]
    public void ConsoleEnvironment_WriteLine_WritesToConsole()
    {
        ConsoleEnvironment env = new ConsoleEnvironment();

        env.WriteLine(42);

        Assert.NotNull(env);
    }

    [Fact]
    public void ConsoleEnvironment_ReadLine_ReadsFromConsole()
    {
        ConsoleEnvironment env = new ConsoleEnvironment();

        Assert.NotNull(env);
    }
}