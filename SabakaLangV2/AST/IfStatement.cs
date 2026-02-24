namespace SabakaLangV2.AST;

public class IfStatement : Statement
{
    public Expression Condition { get; }
    public Statement ThenBranch { get; }
    public Statement? ElseBranch { get; }

    public IfStatement(Expression condition, Statement thenBranch, Statement? elseBranch,
        int line, int column)
        : base(line, column)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }
}
