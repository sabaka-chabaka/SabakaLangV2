namespace SabakaLangV2.AST;

public class WhileStatement : Statement
{
    public Expression Condition { get; }
    public Statement Body { get; }

    public WhileStatement(Expression condition, Statement body,
        int line, int column)
        : base(line, column)
    {
        Condition = condition;
        Body = body;
    }
}