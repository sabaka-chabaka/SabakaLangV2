namespace SabakaLangV2.AST.Expressions;

public class IdentifierExpression : Expression
{
    public string Name { get; }

    public IdentifierExpression(string name, int line, int column)
        : base(line, column)
    {
        Name = name;
    }
}