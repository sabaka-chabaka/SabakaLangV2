namespace SabakaLangV2.Semantics;

public abstract class TypeSymbol
{
    public string Name { get; }

    protected TypeSymbol(string name)
    {
        Name = name;
    }

    public override string ToString() => Name;
}