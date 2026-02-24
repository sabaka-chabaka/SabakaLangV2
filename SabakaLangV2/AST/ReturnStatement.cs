namespace SabakaLangV2.AST;

public class ReturnStatement : Statement
{
    public Expression? Expression { get; }

    public ReturnStatement(Expression? expression, int line, int column)
        : base(line, column)
    {
        Expression = expression;
    }
}