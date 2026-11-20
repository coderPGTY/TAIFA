namespace SharkSharp.Ast
{
    /// <summary>
    /// Интерфейс посетителя для обхода узлов AST (паттерн Visitor).
    /// </summary>
    public interface IAstVisitor
    {
        void VisitIntegerLiteral(IntegerLiteralExpression node);

        void VisitBinary(BinaryExpression node);

        void VisitUnary(UnaryExpression node);

        void VisitFunctionCall(FunctionCallExpression node);

        void VisitExpressionStatement(ExpressionStatement node);

        void VisitPrintStatement(PrintStatement node);

        void VisitBlock(BlockStatement node);

        void VisitMainFunction(MainFunctionDeclaration node);

        void VisitProgram(ProgramNode node);
    }
}