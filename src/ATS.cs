interface INode
{
    string TokenLiteral();
    string OutLine();
}

interface IStatement : INode
{
    void StatementNode();
}

interface IExpression : INode
{
    void ExpressionNode();
}

class Identifier : IExpression
{
    public Token Token { get; set; }
    public Identifier Name { get; set; }
    public string Value { get; set; }
    public void ExpressionNode()
    {

    }

    public string OutLine()
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

    public string OutLine()
    {
        string str = $"{TokenLiteral()}  {Name.OutLine()} ={Value?.OutLine()};";
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

    public string OutLine()
    {
        string str = $"{TokenLiteral()}  ={Value?.OutLine()};";
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
        Statement = new List<IStatement>();
    }
    public List<IStatement> Statement { get; set; }

    public string OutLine()
    {
        string str = string.Empty;
        foreach (var item in Statement)
        {
            str += item.OutLine();
        }
        return str;
    }

    public string TokenLiteral()
    {
        if (Statement.Count > 0)
        {
            return Statement[0].TokenLiteral();
        }
        return "";
    }
}


