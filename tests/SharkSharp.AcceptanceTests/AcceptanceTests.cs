using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SharkSharp.Lexer;
using SharkSharp.Parser;
using Xunit;

namespace SharkSharp.AcceptanceTests;

public class AcceptanceTests
{
    private static readonly string TestDataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData");

    public static TheoryData<string> GetTestCases()
    {
        TheoryData<string> data = new TheoryData<string>();

        if (Directory.Exists(TestDataDirectory))
        {
            string[] files = Directory.GetFiles(TestDataDirectory, "*.shark");
            foreach (string filePath in files)
            {
                data.Add(Path.GetFileNameWithoutExtension(filePath));
            }
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void Interpreter_ShouldProduceCorrectOutput(string testName)
    {
        string sharkFilePath = Path.Combine(TestDataDirectory, $"{testName}.shark");
        string stdoutFilePath = Path.Combine(TestDataDirectory, $"{testName}.stdout");
        string stdinFilePath = Path.Combine(TestDataDirectory, $"{testName}.stdin");

        string sourceCode = File.ReadAllText(sharkFilePath);
        string expectedOutput = File.Exists(stdoutFilePath) ? File.ReadAllText(stdoutFilePath).Trim() : string.Empty;

        List<string> inputs = new List<string>();
        if (File.Exists(stdinFilePath))
        {
            string[] lines = File.ReadAllLines(stdinFilePath);
            inputs = lines.ToList();
        }

        FakeEnvironment env = new FakeEnvironment(inputs);
        SharkSharp.Lexer.Lexer lexer = new SharkSharp.Lexer.Lexer(sourceCode);
        List<Token> tokens = lexer.Tokenize();
        SharkSharp.Parser.Parser parser = new SharkSharp.Parser.Parser(tokens, env);

        parser.ParseProgram();

        string actualOutput = string.Join(Environment.NewLine, env.Outputs);
        Assert.Equal(expectedOutput, actualOutput);
    }
}