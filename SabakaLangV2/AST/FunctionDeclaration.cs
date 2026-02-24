namespace SabakaLangV2.AST;

public class FunctionDeclaration : Statement
{
    public string ReturnTypeName { get; }
    public string Name { get; }
    public List<Parameter> Parameters { get; }
    public BlockStatement Body { get; }

    public FunctionDeclaration(
        string returnTypeName,
        string name,
        List<Parameter> parameters,
        BlockStatement body,
        int line,
        int column)
        : base(line, column)
    {
        ReturnTypeName = returnTypeName;
        Name = name;
        Parameters = parameters;
        Body = body;
    }
}

public class Parameter
{
    public string TypeName { get; }
    public string Name { get; }

    public Parameter(string typeName, string name)
    {
        TypeName = typeName;
        Name = name;
    }
}