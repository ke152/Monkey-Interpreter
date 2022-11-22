interface INode
{
    string TokenLiteral();
    string String();
}

interface IStatement : INode
{
    void StatementNode();
}

interface IExpression : INode
{

}

class Identifier : IExpression
{
    public Token Token { get; set; }
    public Identifier Name { get; set; }
    public string Value { get; set; }

    public string String()
    {
        var str = $"{Value}";
        return str;
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}

class LetStatement : IStatement
{
    public Token Token { get; set; }
    public Identifier Name { get; set; }
    public IExpression Value { get; set; }

    public string String()
    {
        string str = $"{TokenLiteral()}  {Name.String()} ={Value?.String()};";
        return str;
    }

    public void StatementNode()
    {

    }

    public string TokenLiteral()
    {

        return Token.Literal;
    }
}

class ReturnStatement : IStatement
{
    public Token Token = new();
    public IExpression? Value { get; set; }

    public string String()
    {
        string str = $"{TokenLiteral()}  ={Value?.String()};";
        return str;
    }

    public void StatementNode()
    {
        throw new NotImplementedException();
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}

class ExpressionStatement : IStatement
{
    public Token Token = new();
    public IExpression? Expression { get; set; }

    public string String()
    {
        string str = $"{Expression?.String()}";
        return str;
    }

    public void StatementNode()
    {
        throw new NotImplementedException();
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}

internal class AtsProgram : INode
{
    public AtsProgram()
    {
        Statements = new List<IStatement>();
    }
    public List<IStatement> Statements { get; set; }

    public string String()
    {
        string str = string.Empty;
        foreach (var item in Statements)
        {
            str += item.String() + '\n';
        }
        return str;
    }

    public void PrintStaments() => Console.WriteLine(String());

    public string TokenLiteral()
    {
        if (Statements.Count > 0)
        {
            return Statements[0].TokenLiteral();
        }
        return "";
    }
}

internal class IntegerLiteral : IExpression  {

    public Token Token;
    public int Value;

    internal IntegerLiteral(Token token)
    {
        this.Token = token;
    }

    public string String()
    {
        return this.Token.Literal;
    }

    public string TokenLiteral()
    {
        return this.Token.Literal;
    }
}

class PrefixExpression : IExpression
{
    public Token Token;
    public string Operator;
    public IExpression? Right { get; set; }

    public PrefixExpression(Token token, string @operator)
    {
        this.Token = token;
        this.Operator = @operator;
    }

    public string String()
    {
        return $"({Token.Literal} {Right?.String()})";
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}

class InfixExpression : IExpression
{
    public Token Token { get; set; }
    public string Operator { get; set; }
    public IExpression? Right;
    public IExpression? Left;

    public string String()
    {
        var str = $"({Left.String()} {Operator}  {Right?.String()})";
        return str;
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}


