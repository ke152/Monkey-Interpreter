internal class Token
{
    public static readonly Dictionary<string, TokenType> KeyWords = new()
    {
        { "fn", TokenType.FUNCTION },
        { "let", TokenType.LET },
        { "true", TokenType.TRUE },
        { "false", TokenType.FALSE },
        { "if", TokenType.IF },
        { "else", TokenType.ELSE },
        { "return", TokenType.RETURN },
    };

    public TokenType Type;
    public string Literal;

    public Token() : this(TokenType.ILLEGAL, string.Empty)
    {

    }

    public Token(TokenType type, char ch)
    {
        Type = type;
        Literal = ch.ToString();
    }

    public Token(TokenType type, string ch)
    {
        Type = type;
        Literal = ch;
    }

    public void Print()
    {
        if (Type == TokenType.ILLEGAL)
            Console.ForegroundColor = ConsoleColor.Red;

        System.Console.WriteLine($"Token is   {Type},  Value is  {Literal}");

        if (Type == TokenType.ILLEGAL)
            Console.ResetColor();
    }
}

enum TokenType
{
    ILLEGAL,//非法
    EOF,//文件结束
    // 标识符+字面量
    IDENT,// add, foobar, x, y, ... 
    INT,// 1343456 
    // 运算符
    ASSIGN,
    PLUS,
    MINUS,
    // 分隔符
    SLASH,//   /
    BANG, // !
    ASTERISK,// *

    COMMA,//逗号
    SEMICOLON,//分号
    LPAREN,//左小括号
    RPAREN,//右小括号
    LBRACE,//左花括号
    RBRACE,//右花括号
    LT, // <
    GT,// >
    COLON,//:
    LBRACKET,//左中括号
    RBRACKET,//右中括号
    EQ, // ==
    NOT_EQ, // !=
    STRING, // "
    // 关键字
    FUNCTION, // 
    LET, // let
    TRUE,
    FALSE,
    IF,
    ELSE,
    RETURN,
    Unquote,
}
