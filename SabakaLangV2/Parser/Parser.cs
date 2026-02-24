using SabakaLangV2.Lexer;
using SabakaLangV2.AST;
using SabakaLangV2.AST.Expressions;
using Expression = SabakaLangV2.AST.Expression;
using UnaryExpression = SabakaLangV2.AST.UnaryExpression;

namespace SabakaLangV2.Parser;

public class Parser
{
    private readonly List<Token> _tokens;
    private int _position;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    private Token Current => Peek(0);

    private Token Peek(int offset)
    {
        if (_position + offset >= _tokens.Count)
            return _tokens[^1];

        return _tokens[_position + offset];
    }

    private Token Advance()
    {
        var current = Current;
        _position++;
        return current;
    }

    private bool Match(TokenType type)
    {
        if (Current.Type != type)
            return false;

        Advance();
        return true;
    }

    private Token Expect(TokenType type)
    {
        if (Current.Type != type)
            throw new Exception($"Expected {type} but got {Current.Type}");

        return Advance();
    }

    // =========================================================
    // ENTRY
    // =========================================================

    public List<Statement> Parse()
    {
        var statements = new List<Statement>();

        while (Current.Type != TokenType.EndOfFile)
        {
            statements.Add(ParseStatement());
        }

        return statements;
    }

    // =========================================================
    // STATEMENTS
    // =========================================================

    private Statement ParseStatement()
    {
        if (IsTypeKeyword(Current.Type))
            return ParseVariableDeclaration();

        if (Match(TokenType.KeywordReturn))
            return ParseReturn();

        if (Match(TokenType.KeywordIf))
            return ParseIf();

        if (Match(TokenType.KeywordWhile))
            return ParseWhile();

        if (Match(TokenType.LeftBrace))
            return ParseBlock();

        return ParseExpressionStatement();
    }

    private Statement ParseBlock()
    {
        var statements = new List<Statement>();

        while (!Match(TokenType.RightBrace))
        {
            if (Current.Type == TokenType.EndOfFile)
                throw new Exception("Unterminated block");

            statements.Add(ParseStatement());
        }

        return new BlockStatement(statements, Current.Line, Current.Column);
    }

    private Statement ParseVariableDeclaration()
    {
        var typeToken = Advance();
        string typeName = typeToken.Lexeme;

        var nameToken = Expect(TokenType.Identifier);

        Expression? initializer = null;

        if (Match(TokenType.Equal))
        {
            initializer = ParseExpression();
        }

        Expect(TokenType.Semicolon);

        return new VariableDeclaration(
            typeName,
            nameToken.Lexeme,
            initializer,
            typeToken.Line,
            typeToken.Column);
    }

    private Statement ParseReturn()
    {
        Expression? expr = null;

        if (!Match(TokenType.Semicolon))
        {
            expr = ParseExpression();
            Expect(TokenType.Semicolon);
        }

        return new ReturnStatement(expr, Current.Line, Current.Column);
    }

    private Statement ParseIf()
    {
        Expect(TokenType.LeftParen);
        var condition = ParseExpression();
        Expect(TokenType.RightParen);

        var thenBranch = ParseStatement();
        Statement? elseBranch = null;

        if (Match(TokenType.KeywordElse))
        {
            elseBranch = ParseStatement();
        }

        return new IfStatement(condition, thenBranch, elseBranch,
            condition.Line, condition.Column);
    }

    private Statement ParseWhile()
    {
        Expect(TokenType.LeftParen);
        var condition = ParseExpression();
        Expect(TokenType.RightParen);

        var body = ParseStatement();

        return new WhileStatement(condition, body,
            condition.Line, condition.Column);
    }

    private Statement ParseExpressionStatement()
    {
        var expr = ParseExpression();
        Expect(TokenType.Semicolon);

        return new ExpressionStatement(expr, expr.Line, expr.Column);
    }

    private bool IsTypeKeyword(TokenType type)
    {
        return type == TokenType.KeywordInt
            || type == TokenType.KeywordFloat
            || type == TokenType.KeywordBool
            || type == TokenType.KeywordString
            || type == TokenType.KeywordVoid;
    }

    // =========================================================
    // EXPRESSIONS (PRATT)
    // =========================================================

    private Expression ParseExpression(int parentPrecedence = 0)
    {
        Expression left = ParseUnary();

        // ====================================
        // ASSIGN & COMPOUND ASSIGN
        // ====================================

        if (IsAssignmentOperator(Current.Type))
        {
            var opToken = Advance();

            if (!IsAssignable(left))
                throw new Exception("Invalid assignment target");

            var right = ParseExpression();

            // simple =
            if (opToken.Type == TokenType.Equal)
            {
                return new AssignmentExpression(
                    left,
                    right,
                    opToken.Line,
                    opToken.Column);
            }

            // compound
            var binaryOperator = GetBinaryFromCompound(opToken.Type);

            var binary = new BinaryExpression(
                left,
                binaryOperator,
                right,
                opToken.Line,
                opToken.Column);

            return new AssignmentExpression(
                left,
                binary,
                opToken.Line,
                opToken.Column);
        }

        // ====================================
        // BINARY
        // ====================================

        while (true)
        {
            int precedence = GetPrecedence(Current.Type);

            if (precedence == 0 || precedence <= parentPrecedence)
                break;

            var op = Advance();
            var right = ParseExpression(precedence);

            left = new BinaryExpression(
                left,
                op.Type,
                right,
                op.Line,
                op.Column);
        }

        return left;
    }

    private Expression ParseUnary()
    {
        if (Current.Type == TokenType.Bang ||
            Current.Type == TokenType.Minus ||
            Current.Type == TokenType.Increment ||
            Current.Type == TokenType.Decrement)
        {
            var op = Advance();
            var operand = ParseUnary();

            return new UnaryExpression(
                op.Type,
                operand,
                op.Line,
                op.Column);
        }

        return ParsePrimary();
    }

    private Expression ParsePrimary()
    {
        var token = Advance();

        return token.Type switch
        {
            TokenType.IntLiteral =>
                new LiteralExpression(token.Literal, token.Line, token.Column),

            TokenType.FloatLiteral =>
                new LiteralExpression(token.Literal, token.Line, token.Column),

            TokenType.StringLiteral =>
                new LiteralExpression(token.Literal, token.Line, token.Column),

            TokenType.KeywordTrue =>
                new LiteralExpression(true, token.Line, token.Column),

            TokenType.KeywordFalse =>
                new LiteralExpression(false, token.Line, token.Column),

            TokenType.KeywordNull =>
                new LiteralExpression(null, token.Line, token.Column),

            TokenType.Identifier =>
                new IdentifierExpression(token.Lexeme, token.Line, token.Column),

            TokenType.LeftParen => ParseGrouped(),

            _ => throw new Exception($"Unexpected token {token.Type}")
        };
    }

    private Expression ParseGrouped()
    {
        var expr = ParseExpression();
        Expect(TokenType.RightParen);
        return expr;
    }

    private int GetPrecedence(TokenType type)
    {
        return type switch
        {
            TokenType.OrOr => 1,
            TokenType.AndAnd => 2,

            TokenType.EqualEqual or TokenType.BangEqual => 3,

            TokenType.Greater or TokenType.GreaterEqual
                or TokenType.Less or TokenType.LessEqual => 4,

            TokenType.Plus or TokenType.Minus => 5,

            TokenType.Star or TokenType.Slash or TokenType.Percent => 6,

            _ => 0
        };
    }
    
    private bool IsAssignable(Expression expr)
    {
        return expr is IdentifierExpression;
    }
    
    private bool IsAssignmentOperator(TokenType type)
    {
        return type == TokenType.Equal
               || type == TokenType.PlusEqual
               || type == TokenType.MinusEqual
               || type == TokenType.StarEqual
               || type == TokenType.SlashEqual
               || type == TokenType.PercentEqual;
    }
    
    private TokenType GetBinaryFromCompound(TokenType type)
    {
        return type switch
        {
            TokenType.PlusEqual => TokenType.Plus,
            TokenType.MinusEqual => TokenType.Minus,
            TokenType.StarEqual => TokenType.Star,
            TokenType.SlashEqual => TokenType.Slash,
            TokenType.PercentEqual => TokenType.Percent,
            _ => throw new Exception("Not a compound assignment")
        };
    }
}