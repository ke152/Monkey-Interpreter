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
        Statements = new List<IStatement>();
    }
    public List<IStatement> Statements { get; set; }

    public string OutLine()
    {
        string str = string.Empty;
        foreach (var item in Statements)
        {
            str += item.OutLine() + '\n';
        }
        return str;
    }

    public void PrintStaments() => Console.WriteLine(OutLine());

    public string TokenLiteral()
    {
        if (Statements.Count > 0)
        {
            return Statements[0].TokenLiteral();
        }
        return "";
    }
}


