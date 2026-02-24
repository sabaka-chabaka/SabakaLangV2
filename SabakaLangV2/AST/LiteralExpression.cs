namespace SabakaLangV2.AST.Expressions;

public class LiteralExpression : Expression
{
    public object? Value { get; }

    public LiteralExpression(object? value, int line, int column)
        : base(line, column)
    {
        Value = value;
    }
}