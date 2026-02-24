namespace SabakaLangV2.Lexer;

public enum TokenType
{
    // ========================
    // === Special / Control ==
    // ========================
    EndOfFile,
    Unknown,

    // ========================
    // === Identifiers / Literals
    // ========================
    Identifier,

    IntLiteral,
    FloatLiteral,
    StringLiteral,
    BoolLiteral,
    NullLiteral,

    // ========================
    // === Keywords ===========
    // ========================

    // Declarations
    KeywordConst,
    KeywordReturn,
    KeywordClass,
    KeywordStruct,
    KeywordEnum,

    // Flow control
    KeywordIf,
    KeywordElse,
    KeywordSwitch,
    KeywordCase,
    KeywordDefault,
    KeywordWhile,
    KeywordFor,
    KeywordBreak,
    KeywordContinue,

    // Object / OOP
    KeywordThis,
    KeywordSuper,
    KeywordNew,

    // Access modifiers
    KeywordPublic,
    KeywordPrivate,
    KeywordProtected,
    KeywordStatic,

    // Logical keywords
    KeywordTrue,
    KeywordFalse,
    KeywordNull,
    
    // Type keywords
    KeywordInt,
    KeywordFloat,
    KeywordBool,
    KeywordString,
    KeywordVoid,

    // ========================
    // === Operators ==========
    // ========================

    // Arithmetic
    Plus,               // +
    Minus,              // -
    Star,               // *
    Slash,              // /
    Percent,            // %

    // Assignment
    Equal,              // =
    PlusEqual,          // +=
    MinusEqual,         // -=
    StarEqual,          // *=
    SlashEqual,         // /=
    PercentEqual,       // %=

    // Comparison
    EqualEqual,         // ==
    BangEqual,          // !=
    Greater,            // >
    GreaterEqual,       // >=
    Less,               // <
    LessEqual,          // <=

    // Logical
    Bang,               // !
    AndAnd,             // &&
    OrOr,               // ||

    // Bitwise (low-level)
    Ampersand,          // &
    Pipe,               // |
    Caret,              // ^
    Tilde,              // ~
    LeftShift,          // <<
    RightShift,         // >>

    // Unary
    Increment,          // ++
    Decrement,          // --

    // Lambda / arrows
    Arrow,              // =>
    FatArrow,           // ->

    // ========================
    // === Delimiters =========
    // ========================

    LeftParen,          // (
    RightParen,         // )
    LeftBrace,          // {
    RightBrace,         // }
    LeftBracket,        // [
    RightBracket,       // ]

    Comma,              // ,
    Dot,                // .
    Semicolon,          // ;
    Colon,              // :
    DoubleColon,        // ::
    Question,           // ?
    
    // ========================
    // === Built-In funcs =====
    // ========================
    Print,
    Input
}
