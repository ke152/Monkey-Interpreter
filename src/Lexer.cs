internal class Lexer
{
	public string Input;
	public int Position ; //当前字符位置
	public int ReadPosition => Position + 1;//下一个字符位置
	public char Ch; //当前字符

	public Lexer(string input) {
		this.Input = input;
		this.ReadChar();
	}

	//读取字符
	public void ReadChar()
	{
		if (this.ReadPosition >= Input.Length)
		{ //到达input末尾
			this.Ch = (char)0; //文件末尾
		}
		else
		{
			this.Ch = this.Input[this.ReadPosition];
		}
		this.Position++;
	}

	//窥探字符
	public char PeekChar()
	{
		if (this.ReadPosition >= this.Input.Length)
		{
			return (char)0;
		}
		else
		{
			return this.Input[this.ReadPosition];
		}
	}

	public Token NextToken()
	{
		Token? tok;
		SkipWhitespace();
		switch (this.Ch)
		{
			case '=':
				if (PeekChar() == '=') {
					var ch = Ch;
					ReadChar();
					var literal = ch.ToString() + Ch.ToString();
					tok = new Token ( TokenType.EQ, literal);
				}
				else
				{
					tok = new Token(TokenType.ASSIGN, this.Ch);
				}
				break;
			case '+':
				tok = new  Token(TokenType.PLUS, this.Ch);
				break;
			case '-':
				tok = new  Token(TokenType.MINUS, this.Ch);
				break;
			case '!':
				if (PeekChar() == '=') {
					var ch = Ch;
					ReadChar();
					var literal = ch.ToString() + Ch.ToString();
					tok = new Token(TokenType.NOT_EQ, literal);
				}
				else
				{
					tok = new Token(TokenType.BANG, this.Ch);
				}
				break;
			case '/':
				tok = new Token(TokenType.SLASH, this.Ch);
				break;
			case '*':
				tok = new  Token(TokenType.ASTERISK, this.Ch);
				break;
			case '<':
				tok = new  Token(TokenType.LT, this.Ch);
				break;
			case '>':
				tok = new  Token(TokenType.GT, this.Ch);
				break;
			case ';':
				tok = new  Token(TokenType.SEMICOLON, this.Ch);
				break;
			case ':':
				tok = new  Token(TokenType.COLON, this.Ch);
				break;
			case ',':
				tok = new  Token(TokenType.COMMA, this.Ch);
				break;
			case '{':
				tok = new  Token(TokenType.LBRACE, this.Ch);
				break;
			case '}':
				tok = new  Token(TokenType.RBRACE, this.Ch);
				break;
			case '(':
				tok = new  Token(TokenType.LPAREN, this.Ch);
				break;
			case ')':
				tok = new  Token(TokenType.RPAREN, this.Ch);
				break;
			case '[':
				tok = new  Token(TokenType.LBRACKET, this.Ch);
				break;
			case ']':
				tok = new  Token(TokenType.RBRACKET, this.Ch);
				break;
			case (char)0:
				tok = new Token(TokenType.EOF, "");
				break;
			default:
				tok = new Token();

				if (IsLetter(this.Ch))
				{
					tok.Literal = ReadIdentifier();
					tok.Type = LookupIdent(tok.Literal);
					return tok;
				}
				else if (IsDigit(this.Ch))
				{
					tok.Type = TokenType.INT;
					tok.Literal = ReadNumber();
					return tok;
				}
				else
				{
					tok = new Token(TokenType.ILLEGAL, this.Ch);
				}
				break;
		}

		this.ReadChar();
		return tok;
	}

	public string ReadIdentifier()
	{
		var position = Position;
		int count = 0;
		while (IsLetter(this.Ch))
		{
			ReadChar();
			count++;
		}
		return Input.Substring(position, count);
	}

	public bool IsLetter(char ch)
	{
		var x = 'a' <= ch && ch <= 'z' || 'A' <= ch && ch <= 'Z' || ch == '_';
		return x;
	}

	public TokenType LookupIdent(string iddent)
	{
		if (Token.KeyWords.TryGetValue(iddent, out TokenType token))
		{
			return token;
		}
		return TokenType.IDENT;
	}

	public void SkipWhitespace()
	{
		while (Char.IsWhiteSpace(this.Ch) || this.Ch == '\t' || this.Ch == '\n' || this.Ch == '\r')
		{
			ReadChar();
		}
	}

	public string ReadNumber()
	{
		var position = Position;
		int count = 0;
		while (IsDigit(this.Ch))
		{
			ReadChar();
			count++;
		}
		return Input.Substring(position, count);
	}

	public bool IsDigit(char ch)
	{
		return '0' <= ch && ch <= '9';
	}
}
