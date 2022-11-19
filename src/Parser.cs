internal class Parser {
	public Lexer Lexer;
	public Token CurToken = new();
	public Token PeekToken = new();
	public List<string> Errors = new();

	public Parser(Lexer l) {
		this.Lexer = l;

		NextToken();
		NextToken();
	}


	public void NextToken()
	{
		this.CurToken = this.PeekToken;
		this.PeekToken = this.Lexer.NextToken();
	}

	public AtsProgram ParseProgram() {
		AtsProgram program = new();

		while (!CurTokenIs(TokenType.EOF))
		{
			var stmt = ParserStatement();
			if (stmt != null)
			{
				program.Statement.Add(stmt);
			}
			NextToken();
		}
		return program;
	}

	public IStatement? ParserStatement()
	{
		IStatement? statement = null;
		switch (CurToken.Type)
		{
			case TokenType.LET:
				statement = ParserLetStatement();
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

	public LetStatement? ParserLetStatement()
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
}
