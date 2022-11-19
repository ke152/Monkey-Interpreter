internal class Token
{
    public static readonly Dictionary<string, TokenType> KeyWords = new()
    {
        { "fn", TokenType.FUNCTION },
        { "let", TokenType.LET }
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
    SLASH,// \
    ASTERISK,

    COMMA,//逗号
    SEMICOLON,//分号
    LPAREN,//左小括号
    RPAREN,//右小括号
    LBRACE,//左花括号
    RBRACE,//右花括号
    // 关键字
    FUNCTION,
    LET,
    LT,
    GT,
    COLON,
    LBRACKET,
    RBRACKET,
}
