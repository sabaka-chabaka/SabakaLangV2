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