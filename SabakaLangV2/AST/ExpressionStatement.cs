namespace SabakaLangV2.AST;

public class ExpressionStatement : Statement
{
    public Expression Expression { get; }

    public ExpressionStatement(Expression expression, int line, int column)
        : base(line, column)
    {
        Expression = expression;
    }
}