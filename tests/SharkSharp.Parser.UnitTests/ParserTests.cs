namespace SharkSharp.Parser.UnitTests
{
    using System;
    using System.Collections.Generic;

    using SharkSharp.Ast;
    using SharkSharp.Lexer;
    using Xunit;

    public class ParserTests
    {
        public static TheoryData<string, Type> ValidProgramData => new TheoryData<string, Type>
        {
            { "func main() { 42; }", typeof(IntegerLiteralExpression) },
            { "func main() { 10 + 2 * 3; }", typeof(BinaryExpression) },
            { "func main() { -5; }", typeof(UnaryExpression) },
            { "func main() { 10 - 3; }", typeof(BinaryExpression) },
            { "func main() { (2 + 3) * 4; }", typeof(BinaryExpression) },
            { "func main() { 7 / 2; }", typeof(BinaryExpression) },
            { "func main() { -7 % 2; }", typeof(BinaryExpression) },
            { "func main() { abs(-10); }", typeof(FunctionCallExpression) },
            { "func main() { min(5, 10); }", typeof(FunctionCallExpression) },
            { "func main() { max(5, 10); }", typeof(FunctionCallExpression) },
            { "func main() { 10 + 7 * (2 * 3 + 4) + 5 + -5; }", typeof(BinaryExpression) },
            { "func main() { 9223372036854775807; }", typeof(IntegerLiteralExpression) },
        };

        public static TheoryData<string, string> ErrorProgramData => new TheoryData<string, string>
        {
            { "func main() { 42 }", "Ожидался токен Semicolon" },
            { "func test() { }", "Точкой входа должна быть функция 'main'" },
            { "func main() { ( }", "Ожидалось число, скобка или функция" },
        };

        [Theory]
        [MemberData(nameof(ValidProgramData))]
        public void ParseProgram_ValidExpressions_BuildsCorrectAst(string source, Type expectedExpressionType)
        {
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.Tokenize();
            Parser parser = new Parser(tokens);

            ProgramNode ast = parser.ParseProgram();

            Assert.NotNull(ast);
            Assert.IsType<MainFunctionDeclaration>(ast.MainFunction);
            BlockStatement block = ast.MainFunction.Body;
            Assert.Single(block.Statements);

            Statement statement = block.Statements[0];
            ExpressionStatement exprStmt = Assert.IsType<ExpressionStatement>(statement);
            Assert.IsType(expectedExpressionType, exprStmt.Expression);
        }

        [Fact]
        public void ParseProgram_PrintStatement_BuildsPrintNode()
        {
            string source = "func main() { print(42); }";
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.Tokenize();
            Parser parser = new Parser(tokens);

            ProgramNode ast = parser.ParseProgram();

            Statement statement = ast.MainFunction.Body.Statements[0];
            Assert.IsType<PrintStatement>(statement);
        }

        [Theory]
        [MemberData(nameof(ErrorProgramData))]
        public void ParseProgram_InvalidInput_ThrowsParseException(string source, string expectedMessagePart)
        {
            Lexer lexer = new Lexer(source);
            List<Token> tokens = lexer.Tokenize();
            Parser parser = new Parser(tokens);

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