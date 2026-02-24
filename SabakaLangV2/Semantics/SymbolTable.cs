namespace SabakaLangV2.Semantics;

public class SymbolTable
{
    private readonly Dictionary<string, TypeSymbol> _symbols = new();
    private readonly SymbolTable? _parent;
    
    private readonly Dictionary<string, FunctionSymbol> _functions = new();

    public SymbolTable(SymbolTable? parent = null)
    {
        _parent = parent;
    }

    public bool Declare(string name, TypeSymbol type)
    {
        if (_symbols.ContainsKey(name))
            return false;

        _symbols[name] = type;
        return true;
    }

    public TypeSymbol? Lookup(string name)
    {
        if (_symbols.TryGetValue(name, out var type))
            return type;

        return _parent?.Lookup(name);
    }

    public bool DeclareFunction(FunctionSymbol function)
    {
        if (_functions.ContainsKey(function.Name))
            return false;

        _functions[function.Name] = function;
        return true;
    }

    public FunctionSymbol? LookupFunction(string name)
    {
        if (_functions.TryGetValue(name, out var f))
            return f;

        return _parent?.LookupFunction(name);
    }
}