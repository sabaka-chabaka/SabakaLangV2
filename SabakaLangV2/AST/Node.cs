namespace SabakaLangV2.AST;

public abstract class Node
{
    public int Line { get; }
    public int Column { get; }

    protected Node(int line, int column)
    {
        Line = line;
        Column = column;
    }
}

public abstract class Expression : Node
{
    protected Expression(int line, int column)
        : base(line, column) { }
}

public abstract class Statement : Node
{
    protected Statement(int line, int column)
        : base(line, column) { }
}