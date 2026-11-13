namespace SharkSharp.Parser.UnitTests
{
    using System;
    using System.Collections.Generic;

    using SharkSharp.Lexer;
    using SharkSharp.Parser;
    using Xunit;

    public class ParserTests
    {
        public static TheoryData<string, long> ValidProgramData => new TheoryData<string, long>
        {
            { "func main() { 42; }", 42 },
            { "func main() { 10 + 2 * 3; }", 16 },
            { "func main() { -5; }", -5 },
            { "func main() { 10 + -5; }", 5 },
            { "func main() { (2 + 3) * 4; }", 20 },
            { "func main() { 7 / 2; }", 3 },
            { "func main() { -7 % 2; }", -1 },
            { "func main() { abs(-10); }", 10 },
            { "func main() { min(5, 10); }", 5 },
            { "func main() { max(5, 10); }", 10 },
            { "func main() { 10 + 7 * (2 * 3 + 4) + 5 + -5; }", 80 },
            { "func main() { 9223372036854775807; }", 9223372036854775807 },
        };

        public static TheoryData<string, string> ErrorProgramData => new TheoryData<string, string>
        {
            { "func main() { 42 }", "Ожидался токен Semicolon" },
            { "func main() { 5 / 0; }", "Деление на ноль" },
            { "func main() { unknown(); }", "Неизвестная функция" },
            { "func test() { }", "Точкой входа должна быть функция 'main'" },
            { "func main() { 5 + print(1); }", "Неизвестная функция 'print'" },
            { "func main() { 9223372036854775807 + 1; }", "вне диапазона" },
        };

        [Theory]
        [MemberData(nameof(ValidProgramData))]
        public void ParseProgram_ValidExpressions_EvaluatesCorrectly(string source, long expectedLastValue)
        {
            SharkSharp.Lexer.Lexer lexer = new SharkSharp.Lexer.Lexer(source);
            List<Token> tokens = lexer.Tokenize();
            SharkSharp.Parser.Parser parser = new SharkSharp.Parser.Parser(tokens, new FakeEnvironment(new List<string>()));

            long actualLastValue = parser.ParseProgram();

            Assert.Equal(expectedLastValue, actualLastValue);
        }

        [Theory]
        [MemberData(nameof(ErrorProgramData))]
        public void ParseProgram_InvalidInput_ThrowsParseException(string source, string expectedMessagePart)
        {
            SharkSharp.Lexer.Lexer lexer = new SharkSharp.Lexer.Lexer(source);
            List<Token> tokens = lexer.Tokenize();
            SharkSharp.Parser.Parser parser = new SharkSharp.Parser.Parser(tokens, new FakeEnvironment(new List<string>()));

            ParseException exception = Assert.Throws<ParseException>(() => parser.ParseProgram());
            Assert.Contains(expectedMessagePart, exception.Message);
        }

        [Fact]
        public void ParseException_PositionConstructor_FormatsMessage()
        {
            SourcePosition position = new SourcePosition(5, 10);
            ParseException exception = new ParseException("error", position);
            Assert.Contains("error", exception.Message);
            Assert.Contains("5", exception.Message);
            Assert.Contains("10", exception.Message);
            Assert.Equal(5, exception.Position.Line);
            Assert.Equal(10, exception.Position.Column);
        }
    }
}