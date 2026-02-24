namespace SabakaLangV2.AST;

public class BlockStatement : Statement
{
    public List<Statement> Statements { get; }

    public BlockStatement(List<Statement> statements, int line, int column)
        : base(line, column)
    {
        Statements = statements;
    }
}