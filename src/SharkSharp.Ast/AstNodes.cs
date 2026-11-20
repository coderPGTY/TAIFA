#pragma warning disable SA1402, SA1649
namespace SharkSharp.Ast
{
    using System.Collections.Generic;

    using SharkSharp.Lexer;

    /// <summary>
    /// Базовый абстрактный класс для всех узлов AST.
    /// </summary>
    public abstract record AstNode
    {
        public abstract void Accept(IAstVisitor visitor);
    }

    /// <summary>
    /// Базовый класс для узлов выражений.
    /// </summary>
    public abstract record Expression : AstNode;

    /// <summary>
    /// Базовый класс для узлов инструкций.
    /// </summary>
    public abstract record Statement : AstNode;

    /// <summary>
    /// Базовый класс для узлов объявлений.
    /// </summary>
    public abstract record Declaration : AstNode;

    public record IntegerLiteralExpression(long Value) : Expression
    {
        public override void Accept(IAstVisitor visitor) => visitor.VisitIntegerLiteral(this);
    }

    public record BinaryExpression(Expression Left, TokenType Operator, Expression Right) : Expression
    {
        public override void Accept(IAstVisitor visitor) => visitor.VisitBinary(this);
    }

    public record UnaryExpression(TokenType Operator, Expression Operand) : Expression
    {
        public override void Accept(IAstVisitor visitor) => visitor.VisitUnary(this);
    }

    public record FunctionCallExpression(string Name, List<Expression> Arguments) : Expression
    {
        public override void Accept(IAstVisitor visitor) => visitor.VisitFunctionCall(this);
    }

    public record ExpressionStatement(Expression Expression) : Statement
    {
        public override void Accept(IAstVisitor visitor) => visitor.VisitExpressionStatement(this);
    }

    public record PrintStatement(Expression Expression) : Statement
    {
        public override void Accept(IAstVisitor visitor) => visitor.VisitPrintStatement(this);
    }

    public record BlockStatement(List<Statement> Statements) : Statement
    {
        public override void Accept(IAstVisitor visitor) => visitor.VisitBlock(this);
    }

    public record MainFunctionDeclaration(BlockStatement Body) : Declaration
    {
        public override void Accept(IAstVisitor visitor) => visitor.VisitMainFunction(this);
    }

    public record ProgramNode(MainFunctionDeclaration MainFunction) : AstNode
    {
        public override void Accept(IAstVisitor visitor) => visitor.VisitProgram(this);
    }
}