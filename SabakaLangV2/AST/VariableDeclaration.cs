namespace SabakaLangV2.AST;

public class VariableDeclaration : Statement
{
    public string TypeName { get; }
    public string Name { get; }
    public Expression? Initializer { get; }

    public VariableDeclaration(
        string typeName,
        string name,
        Expression? initializer,
        int line,
        int column)
        : base(line, column)
    {
        TypeName = typeName;
        Name = name;
        Initializer = initializer;
    }
}