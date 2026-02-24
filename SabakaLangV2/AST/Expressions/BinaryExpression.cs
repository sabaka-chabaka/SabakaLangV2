using SabakaLangV2.Lexer;

namespace SabakaLangV2.AST.Expressions;

public class BinaryExpression : Expression
{
    public Expression Left { get; }
    public TokenType Operator { get; }
    public Expression Right { get; }

    public BinaryExpression(
        Expression left,
        TokenType op,
        Expression right,
        int line,
        int column)
        : base(line, column)
    {
        Left = left;
        Operator = op;
        Right = right;
    }
}