namespace SabakaLangV2.Lexer;

public class Token
{
    public TokenType Type { get; }
    public object? Literal { get; set; }
    public string Lexeme { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
    public int TokenStart { get; set; }
    public int TokenEnd { get; set; }
    
    public Token(TokenType type, object? literal, int line, int column, int tokenStart, int tokenEnd, string lexeme)
    {
        Type = type;
        Literal = literal;
        Line = line;
        Column = column;
        TokenStart = tokenStart;
        TokenEnd = tokenEnd;
        Lexeme = lexeme;
    }

    public override string ToString()
    {
        return Literal != null
            ? $"{Type}({Literal})"
            : Type.ToString();
    }
}