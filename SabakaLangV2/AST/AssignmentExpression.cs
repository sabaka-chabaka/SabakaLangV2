namespace SabakaLangV2.AST;

public class AssignmentExpression : Expression
{
    public Expression Target { get; }
    public Expression Value { get; }

    public AssignmentExpression(
        Expression target,
        Expression value,
        int line,
        int column)
        : base(line, column)
    {
        Target = target;
        Value = value;
    }
}