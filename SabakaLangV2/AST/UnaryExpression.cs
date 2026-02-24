using SabakaLangV2.Lexer;

namespace SabakaLangV2.AST;

public class UnaryExpression : Expression
{
    public TokenType Operator { get; }
    public Expression Operand { get; }

    public UnaryExpression(TokenType op, Expression operand, int line, int column)
        : base(line, column)
    {
        Operator = op;
        Operand = operand;
    }
}