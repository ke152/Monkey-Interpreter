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
    public IExpression? ReturnValue { get; set; }

    public string String()
    {
        string str = $"{TokenLiteral()}  ={ReturnValue?.String()};";
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

internal class AstProgram : INode
{
    public AstProgram()
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

class IfExpression : IExpression
{
    public Token Token;
    public IExpression? Condition;
    public BlockStatement? Consequence;
    public BlockStatement? Alternative;

    public IfExpression(Token token)
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
    public List<Identifier> Parameter = new();

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
    public IExpression Function;
    public List<IExpression?> Arguments = new();

    public CallExpression(Token token, IExpression func, List<IExpression?> list)
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

internal class StringLiteral : IExpression
{

    public Token Token;
    public string Value;

    internal StringLiteral(Token token, string value)
    {
        Token = token;
        Value = value;
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

class ArrayLiteral : IExpression
{
    public Token Token;
    public List<IExpression?> Element = new();

    public ArrayLiteral(Token token)
    {
        this.Token = token;
    }

    public string String()
    {
        var ele = new List<string?>();
        if (Element != null)
        {
            foreach (var item in Element)
            {
                ele.Add(item?.String());
            }
        }
        return $"[{string.Join(",", ele)}]";
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}

class IndexExpression : IExpression
{
    public Token Token;
    public IExpression? Left;

    public IExpression? Index;

    public IndexExpression(Token token, IExpression? left)
    {
        this.Token= token;
        this.Left = left;
    }

    public string String()
    {
        return $"({Left?.String()}[{Index?.String()}])";
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}

class HashLiteral : IExpression
{
    public Token Token;//'{'词法单元
    public Dictionary<IExpression, IExpression?> Pairs = new();

    public HashLiteral(Token token)
    {
        this.Token = token;
    }

    public string String()
    {
        var str = new List<string>();
        foreach (var item in Pairs.Values)
        {
            str.Add(item.String());
        }
        return $"{{{string.Join(",", str)}}}";
    }

    public string TokenLiteral()
    {
        return Token.Literal;
    }
}


internal class AstModify
{
    public delegate INode? ModifierFunc(INode? node);

    public static INode Modify(INode? node, ModifierFunc modifier)
    {
        switch (node)
        {
            case AstProgram e:
                for (int i = 0; i < e.Statements.Count; i++)
                {
                    e.Statements[i] = (IStatement)Modify(e.Statements[i], modifier);
                }
                break;
            case ExpressionStatement e:
                e.Expression = (IExpression)Modify(e.Expression, modifier);
                break;
            case InfixExpression e:
                e.Left = (IExpression)Modify(e.Left, modifier);
                e.Right = (IExpression)Modify(e.Right, modifier);
                break;
            case PrefixExpression e:
                e.Right = (IExpression)Modify(e.Right, modifier);
                break;
            case IndexExpression e:
                e.Left = (IExpression)Modify(e.Left, modifier);
                e.Index = (IExpression)Modify(e.Index, modifier);
                break;
            case IfExpression e:
                e.Condition = (IExpression)Modify(e.Condition, modifier);
                e.Consequence = (BlockStatement)Modify(e.Consequence, modifier);
                if (e.Alternative != null)
                {
                    e.Alternative = (BlockStatement)Modify(e.Alternative, modifier);
                }
                break;
            case BlockStatement e:
                for (int i = 0; i < e.Statements.Count; i++)
                {
                    e.Statements[i] = (IStatement)Modify(e.Statements[i], modifier);
                }
                break;
            case ReturnStatement e:
                e.ReturnValue = (IExpression)Modify(e.ReturnValue, modifier);
                break;
            case LetStatement e:
                e.Value = (IExpression)Modify(e.Value, modifier);
                break;
            case FunctionLiteral e:
                for (int i = 0; i < e.Parameter.Count; i++)
                {
                    e.Parameter[i] = (Identifier)Modify(e.Parameter[i], modifier);
                }
                e.Body = (BlockStatement)Modify(e.Body, modifier);
                break;
            case ArrayLiteral e:
                for (int i = 0; i < e.Element.Count; i++)
                {
                    e.Element[i] = (IExpression?)Modify(e.Element[i], modifier);
                }
                break;
            case HashLiteral e:
                var newPairs = new Dictionary<IExpression, IExpression?>();
                foreach (var pair in e.Pairs)
                {
                    var newKey = (IExpression)Modify(pair.Key, modifier);
                    var newVal = (IExpression?)Modify(pair.Value, modifier);
                    newPairs.Add(newKey, newVal);
                }
                break;
        }

        return modifier(node);
    }
}

class MacroLiteral : IExpression
{
    public Token Token;//macro词法单元
    public List<Identifier> Parameter = new();

    public BlockStatement? Body;

    public MacroLiteral(Token token)
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