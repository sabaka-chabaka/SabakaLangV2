namespace SabakaLangV2.Semantics;

public class FunctionSymbol
{
    public string Name { get; }
    public TypeSymbol ReturnType { get; }
    public List<TypeSymbol> Parameters { get; }

    public FunctionSymbol(string name, TypeSymbol returnType, List<TypeSymbol> parameters)
    {
        Name = name;
        ReturnType = returnType;
        Parameters = parameters;
    }
}