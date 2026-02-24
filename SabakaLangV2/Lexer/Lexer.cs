using System.Globalization;
using System.Text;

namespace SabakaLangV2.Lexer;

public class Lexer
{
    private readonly string _text;
    private readonly List<Token> _tokens = new();

    private int _position;
    private int _line = 1;
    private int _column = 1;

    public Lexer(string text)
    {
        _text = text;
    }

    private bool IsAtEnd => _position >= _text.Length;

    private char Current => IsAtEnd ? '\0' : _text[_position];
    private char Peek(int offset = 1)
        => _position + offset >= _text.Length ? '\0' : _text[_position + offset];

    private void Advance()
    {
        if (Current == '\n')
        {
            _line++;
            _column = 1;
        }
        else
        {
            _column++;
        }

        _position++;
    }

    private bool Match(char expected)
    {
        if (IsAtEnd || Current != expected)
            return false;

        Advance();
        return true;
    }

    private void AddToken(TokenType type, object? literal, int startPos, int startColumn)
    {
        string lexeme = _text[startPos.._position];

        _tokens.Add(new Token(
            type,
            literal,
            _line,
            startColumn,
            startPos,
            _position,
            lexeme));
    }

    public List<Token> Tokenize()
    {
        while (!IsAtEnd)
        {
            SkipWhitespaceAndComments();

            if (IsAtEnd)
                break;

            int startPos = _position;
            int startColumn = _column;

            char c = Current;

            if (char.IsLetter(c) || c == '_')
            {
                ReadIdentifier(startPos, startColumn);
            }
            else if (char.IsDigit(c))
            {
                ReadNumber(startPos, startColumn);
            }
            else
            {
                ReadOperatorOrDelimiter(startPos, startColumn);
            }
        }

        _tokens.Add(new Token(TokenType.EndOfFile, null, _line, _column, _position, _position, ""));
        return _tokens;
    }

    // ========================
    // Whitespace / Comments
    // ========================

    private void SkipWhitespaceAndComments()
    {
        while (true)
        {
            if (char.IsWhiteSpace(Current))
            {
                Advance();
            }
            else if (Current == '/' && Peek() == '/')
            {
                while (Current != '\n' && !IsAtEnd)
                    Advance();
            }
            else if (Current == '/' && Peek() == '*')
            {
                Advance(); Advance();

                while (!(Current == '*' && Peek() == '/') && !IsAtEnd)
                    Advance();

                if (!IsAtEnd)
                {
                    Advance();
                    Advance();
                }
            }
            else break;
        }
    }

    // ========================
    // Identifiers / Keywords
    // ========================

    private void ReadIdentifier(int startPos, int startColumn)
    {
        while (char.IsLetterOrDigit(Current) || Current == '_')
            Advance();

        string text = _text[startPos.._position];

        TokenType type = text switch
        {
            "const" => TokenType.KeywordConst,
            "return" => TokenType.KeywordReturn,
            "class" => TokenType.KeywordClass,
            "struct" => TokenType.KeywordStruct,
            "enum" => TokenType.KeywordEnum,

            "if" => TokenType.KeywordIf,
            "else" => TokenType.KeywordElse,
            "switch" => TokenType.KeywordSwitch,
            "case" => TokenType.KeywordCase,
            "default" => TokenType.KeywordDefault,
            "while" => TokenType.KeywordWhile,
            "for" => TokenType.KeywordFor,
            "break" => TokenType.KeywordBreak,
            "continue" => TokenType.KeywordContinue,

            "this" => TokenType.KeywordThis,
            "super" => TokenType.KeywordSuper,
            "new" => TokenType.KeywordNew,

            "public" => TokenType.KeywordPublic,
            "private" => TokenType.KeywordPrivate,
            "protected" => TokenType.KeywordProtected,
            "static" => TokenType.KeywordStatic,

            "true" => TokenType.KeywordTrue,
            "false" => TokenType.KeywordFalse,
            "null" => TokenType.KeywordNull,
            
            "print" => TokenType.Print,
            "input" => TokenType.Input,
            
            "int" => TokenType.KeywordInt,
            "float" => TokenType.KeywordFloat,
            "bool" => TokenType.KeywordBool,
            "string" => TokenType.KeywordString,
            "void" => TokenType.KeywordVoid,

            _ => TokenType.Identifier
        };

        AddToken(type, null, startPos, startColumn);
    }

    // ========================
    // Numbers
    // ========================

    private void ReadNumber(int startPos, int startColumn)
    {
        while (char.IsDigit(Current))
            Advance();

        bool isFloat = false;

        if (Current == '.' && char.IsDigit(Peek()))
        {
            isFloat = true;
            Advance();

            while (char.IsDigit(Current))
                Advance();
        }

        string numberText = _text[startPos.._position];

        if (isFloat)
        {
            AddToken(TokenType.FloatLiteral,
                float.Parse(numberText, CultureInfo.InvariantCulture),
                startPos, startColumn);
        }
        else
        {
            AddToken(TokenType.IntLiteral,
                int.Parse(numberText),
                startPos, startColumn);
        }
    }

    // ========================
    // Operators / Delimiters
    // ========================

    private void ReadOperatorOrDelimiter(int startPos, int startColumn)
    {
        char c = Current;
        Advance();

        TokenType type = c switch
        {
            '+' => Match('=') ? TokenType.PlusEqual :
                   Match('+') ? TokenType.Increment :
                   TokenType.Plus,

            '-' => Match('=') ? TokenType.MinusEqual :
                   Match('-') ? TokenType.Decrement :
                   Match('>') ? TokenType.FatArrow :
                   TokenType.Minus,

            '*' => Match('=') ? TokenType.StarEqual : TokenType.Star,
            '/' => Match('=') ? TokenType.SlashEqual : TokenType.Slash,
            '%' => Match('=') ? TokenType.PercentEqual : TokenType.Percent,

            '=' => Match('=') ? TokenType.EqualEqual :
                   Match('>') ? TokenType.Arrow :
                   TokenType.Equal,

            '!' => Match('=') ? TokenType.BangEqual : TokenType.Bang,

            '>' => Match('=') ? TokenType.GreaterEqual :
                   Match('>') ? TokenType.RightShift :
                   TokenType.Greater,

            '<' => Match('=') ? TokenType.LessEqual :
                   Match('<') ? TokenType.LeftShift :
                   TokenType.Less,

            '&' => Match('&') ? TokenType.AndAnd : TokenType.Ampersand,
            '|' => Match('|') ? TokenType.OrOr : TokenType.Pipe,

            '^' => TokenType.Caret,
            '~' => TokenType.Tilde,

            '(' => TokenType.LeftParen,
            ')' => TokenType.RightParen,
            '{' => TokenType.LeftBrace,
            '}' => TokenType.RightBrace,
            '[' => TokenType.LeftBracket,
            ']' => TokenType.RightBracket,

            ',' => TokenType.Comma,
            '.' => TokenType.Dot,
            ';' => TokenType.Semicolon,

            ':' => Match(':') ? TokenType.DoubleColon : TokenType.Colon,

            '?' => TokenType.Question,

            '"' => ReadString(startPos, startColumn),

            _ => TokenType.Unknown
        };

        if (type != TokenType.Unknown && c != '"')
            AddToken(type, null, startPos, startColumn);
    }

    // ========================
    // Strings
    // ========================

    private TokenType ReadString(int startPos, int startColumn)
    {
        var sb = new StringBuilder();

        while (Current != '"' && !IsAtEnd)
        {
            if (Current == '\\')
            {
                Advance();

                sb.Append(Current switch
                {
                    'n' => '\n',
                    't' => '\t',
                    '"' => '"',
                    '\\' => '\\',
                    _ => Current
                });
            }
            else
            {
                sb.Append(Current);
            }

            Advance();
        }

        Advance(); // closing "

        _tokens.Add(new Token(
            TokenType.StringLiteral,
            sb.ToString(),
            _line,
            startColumn,
            startPos,
            _position,
            _text[startPos.._position]));

        return TokenType.Unknown;
    }
}