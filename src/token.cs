internal struct Token
{
    public TokenType Type;
    public string Literal;
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
    // 分隔符
    COMMA,//逗号
    SEMICOLON,//分号
    LPAREN,//左小括号
    RPAREN,//右小括号
    LBRACE,//左花括号
    RBRACE,//右花括号
    // 关键字
    FUNCTION,
    LET,
}
