interface INode
{
    string TokenLiteral();
    string String();
}

interface IStatement : INode
{

}

interface IExpression : INode
{

}

class Identifier : IExpression
{
    public Token? Token;
    public Identifier? Name;
    public string? Value;

    public string String()
    {
        var str = $"{Value}";
        return str;
    }

    public string TokenLiteral()
    {
        var ret = Token?.Literal;
        return ret ?? string.Empty;
    }
}

class LetStatement : IStatement
{
    public Token? Token;
    public Identifier? Name;
    public IExpression? Value;

    public string String()
    {
        string str = $"{TokenLiteral()}  {Name?.String()} = {Value?.String()};";
        return str;
    }

    public void StatementNode()
    {

    }

    public string TokenLiteral()
    {
        var ret = Token?.Literal;
        return ret ?? string.Empty;
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
        return string.Empty;
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

internal class BooleanExpression : IExpression
{
    public Token Token;
    public bool Value;

    internal BooleanExpression(Token token, bool value)
    {
        this.Token = token;
        this.Value = value;
    }
    public void ExpressionNode()
    {
        throw new NotImplementedException();
    }

    public string String()
    {
        return Token.Literal;
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}

class BlockStatement : IStatement
{
    public Token Token;

    public readonly List<IStatement> Statements = new();

    public BlockStatement(Token curToken)
    {
        this.Token= curToken;
    }

    public string String()
    {
        var str = string.Empty;
        str += "{\n";
        foreach (var item in Statements)
        {
            str += "\t" + item?.String();
        }
        str += "}\n";
        return str;
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}

class IFExpression : IExpression
{
    public Token Token;
    public IExpression? Condition;
    public BlockStatement? Consequence;
    public BlockStatement? Alternative;

    public IFExpression(Token token)
    {
        this.Token = token;
    }

    public string String()
    {
        var str = $"if ({Condition?.String()}) {{\n" +
            $"{Consequence?.String()}\n}} else {{\n" +
             $"{Alternative?.String()}\n}}\n";
        return str;
    }

    public string TokenLiteral()
    {
        return Token.Literal;
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
    public Token? Token;
    public string? Operator;
    public IExpression? Right;
    public IExpression? Left;

    public string String()
    {
        var str = $"({Left?.String()} {Operator}  {Right?.String()})";
        return str;
    }

    public string TokenLiteral()
    {
        var ret = Token?.Literal;
        return ret ?? string.Empty;
    }
}
class FunctionLiteral : IExpression
{
    public Token Token;
    public List<Identifier>? Parameter;

    public BlockStatement? Body;

    public FunctionLiteral(Token token)
    {
        this.Token = token;
    }

    public string String()
    {
        string str = string.Empty;
        string param = string.Empty;

        if (Parameter != null)
        {
            foreach (var item in Parameter)
            {
                param += item?.String();
            }
        }

        str += TokenLiteral();
        str += $"({string.Join(",", param)})\n{{\n{Body?.String()}\n}}\n";
        return str;

    }
    public string TokenLiteral()
    {
        return Token.Literal;
    }

}

class CallExpression : IExpression
{
    public Token Token;
    public IExpression? Function;
    public List<IExpression?>? Arguments;

    public CallExpression(Token token, IExpression? func, List<IExpression?>? list)
    {
        this.Token = token;
        this.Function = func;
        this.Arguments = list;
    }

    public string String()
    {
        string str = string.Empty, param = string.Empty;

        if (Arguments != null)
        {
            foreach (var item in Arguments)
            {
                param += item?.String();
            }
        }
        
        str += Function?.String() ?? string.Empty;
        str += $"({string.Join(",", param)})";
        return str;
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}

