#region Priority
internal class Precedence
{
	public const int LOWEST = 1;
	public const int EQUALS = 2;      // == !=
	public const int LESSGREATER = 3; // > or <
	public const int SUM = 4;         // + -
	public const int PRODUCT = 5;     // * /
	public const int PREFIX = 6;      // -X or !X
	public const int CALL = 7;       // myFunction(X)

	public static readonly Dictionary<TokenType, int> TypeDict = new()
		{
			{ TokenType.EQ, Precedence.EQUALS },
			{ TokenType.NOT_EQ, Precedence.EQUALS },
			{ TokenType.LT, Precedence.LESSGREATER },
			{ TokenType.GT, Precedence.LESSGREATER },
			{ TokenType.PLUS, Precedence.SUM },
			{ TokenType.MINUS, Precedence.SUM },
			{ TokenType.SLASH, Precedence.PRODUCT },
			{ TokenType.ASTERISK, Precedence.PRODUCT },
			{TokenType.FUNCTION, Precedence.CALL }
		};
}
#endregion

internal class Parser
{
 

    public Lexer Lexer;
	public Token CurToken = new();
	public Token PeekToken = new();
	public List<string> Errors = new();

	public Parser(Lexer l)
	{
		this.Lexer = l;

		RegisterPrefix(TokenType.IDENT, this.ParseIdentifier);
		RegisterPrefix(TokenType.INT, this.ParseIdentifier);
		RegisterPrefix(TokenType.BANG, this.ParsePrefixExpression);
		RegisterPrefix(TokenType.MINUS, this.ParsePrefixExpression);

		Registerinfix(TokenType.PLUS, ParseInfixExpression);
		Registerinfix(TokenType.MINUS, ParseInfixExpression);
		Registerinfix(TokenType.SLASH, ParseInfixExpression);
		Registerinfix(TokenType.ASTERISK, ParseInfixExpression);
		Registerinfix(TokenType.EQ, ParseInfixExpression);
		Registerinfix(TokenType.NOT_EQ, ParseInfixExpression);
		Registerinfix(TokenType.LT, ParseInfixExpression);
		Registerinfix(TokenType.GT, ParseInfixExpression);
		//Registerinfix(TokenType.FUNCTION, ParseCallExpression);

		NextToken();
		NextToken();
	}

	public AtsProgram ParseProgram()
	{
		AtsProgram program = new();

		while (!CurTokenIs(TokenType.EOF))
		{
			var stmt = ParseStatement();
			if (stmt != null)
			{
				program.Statements.Add(stmt);
			}
			NextToken();
		}
		return program;
	}

	#region  token

	public void NextToken()
	{
		this.CurToken = this.PeekToken;
		this.PeekToken = this.Lexer.NextToken();
	}

	public bool PeekTokenIs(TokenType token)
	{
		return PeekToken?.Type == token;
	}

	public bool CurTokenIs(TokenType token)
	{
		return CurToken?.Type == token;
	}

	public bool ExpectPeek(TokenType token)
	{
		if (PeekTokenIs(token))
		{
			NextToken();
			return true;
		}
		else
		{
			return false;
		}
	}

	public void PeekError(TokenType token)
	{
		var error = $"expected next token to be {token}, got {PeekToken.Type} instead ";
		Console.WriteLine(error);
		Errors.Add(error);
	}

	public int PeekPrecedence()
	{
		if (Precedence.TypeDict.TryGetValue(PeekToken.Type, out int precedence))
		{
			return precedence;
		}
		return Precedence.LOWEST;
	}

	public int CurPrecedence()
	{
		if (Precedence.TypeDict.TryGetValue(CurToken.Type, out int precedence))
		{
			return precedence;
		}
		return Precedence.LOWEST;
	}
	#endregion

	#region Parse Statement and Expression

	public void NoPreExpressionError(TokenType token)
	{
		var error = $"no prefix parse function for {token}";
		Console.WriteLine(error);
		this.Errors.Add(error);
	}

	public IStatement? ParseStatement()
	{
		IStatement? statement = null;
		switch (CurToken.Type)
		{
			case TokenType.LET:
				statement = ParseLetStatement();
				break;
			case TokenType.RETURN:
				statement = ParserRuturnStatement();
				break;
			default:
				statement = ParseExpressionStatement();
				break;
		}
		return statement;
	}

	public LetStatement? ParseLetStatement()
	{
		var stmt = new LetStatement() { Token = CurToken };
		if (!ExpectPeek(TokenType.IDENT))
		{
			//CurError(stmt);
			//PeekError(TokenEnum.IDENT);
			return null;
		}
		stmt.Name = new Identifier() { Token = CurToken, Value = CurToken.Literal };
		if (!ExpectPeek(TokenType.ASSIGN))
		{
			//CurError(stmt);
			//PeekError(TokenEnum.ASSIGN);
			return null;
		}
		//NextToken();
		//stmt.Value = ParserExpression();
		//if (PeekTokenIs(TokenEnum.SEMICOLON))
		//{
		//	NextToken();
		//}
		while (!CurTokenIs(TokenType.SEMICOLON))
		{
			NextToken();
		}
		return stmt;
	}
	public ReturnStatement ParserRuturnStatement()
	{
		var stmt = new ReturnStatement() { Token = CurToken };
		NextToken();
		//stmt.Value = ParseExpression();
		//if (PeekTokenIs(TokenEnum.SEMICOLON))
		//{
		//	NextToken();
		//}
		while (!CurTokenIs(TokenType.SEMICOLON))
		{
			NextToken();
		}
		return stmt;
	}

	public ExpressionStatement ParseExpressionStatement()
	{
		ExpressionStatement stmt = new ExpressionStatement() { Token = CurToken };
		stmt.Expression = ParseExpression();
		if (PeekTokenIs(TokenType.SEMICOLON))
		{
			NextToken();
		}
		return stmt;
	}

	#endregion

	#region dict of parser function and statement type

	public readonly Dictionary<TokenType, Func<IExpression>> PrefixParseFns = new();
	public readonly Dictionary<TokenType, Func<IExpression, IExpression>> InfixParseFns = new();
	public void RegisterPrefix(TokenType token, Func<IExpression> fn)
	{
		PrefixParseFns[token] = fn;
	}
	public void Registerinfix(TokenType token, Func<IExpression, IExpression> fn)
	{
		InfixParseFns[token] = fn;
	}

	IExpression? ParseExpression(int precedence = Precedence.LOWEST)
	{
		PrefixParseFns.TryGetValue(CurToken.Type, out Func<IExpression>? prefix);
		if (prefix == null)
		{
			NoPreExpressionError(CurToken.Type);
			return null;
		}

		//使用前缀解析函数
		var leftExp = prefix();
        while (!PeekTokenIs(TokenType.SEMICOLON) && precedence < PeekPrecedence())
        {
            var infix = InfixParseFns[PeekToken.Type];
            if (infix == null)
            {
                return leftExp;
            }
            NextToken();
            leftExp = infix(leftExp);
        }
        return leftExp;
	}

	public IExpression ParseIdentifier()
	{
		return new Identifier { Token = CurToken, Value = CurToken.Literal };
	}

	public IExpression? ParseIntegerLiteral()
	{
		var lit = new IntegerLiteral(CurToken);

		if (int.TryParse(this.CurToken.Literal, out int value) == false)
		{
			Console.WriteLine($"could not parse {this.CurToken.Literal} as integer");
			return null;
		}

		lit.Value = value;

		return lit;
	}

	public IExpression ParsePrefixExpression()
	{
		var exp = new PrefixExpression(CurToken, CurToken.Literal);
		NextToken();
		exp.Right = ParseExpression(Precedence.PREFIX);
		return exp;

	}

	public IExpression ParseInfixExpression(IExpression expression)
	{
		var exp = new InfixExpression() { Token = CurToken, Operator = CurToken.Literal, Left = expression };

		var precedence = CurPrecedence();
		NextToken();
		exp.Right = ParseExpression(precedence);
		return exp;

	}
	#endregion
}
