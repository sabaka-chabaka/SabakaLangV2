namespace SabakaLangV2.AST;

public class CallExpression : Expression
{
    public Expression Target { get; }
    public List<Expression> Arguments { get; }

    public CallExpression(
        Expression target,
        List<Expression> arguments,
        int line,
        int column)
        : base(line, column)
    {
        Target = target;
        Arguments = arguments;
    }
}