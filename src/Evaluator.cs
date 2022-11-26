internal class Evaluator
{
    public IMonkeyObject? Eval(INode? node)
    {
        if (node == null) return null;

        switch (node)
        {
            case AtsProgram n:
                return EvalProgram(n.Statements);
            case ExpressionStatement n:
                return Eval(n);
            case IntegerLiteral n:
                return new MonkeyInteger(n.Value);
            case BooleanExpression n:
                return NativeBoolToBooleanObject(n.Value);
            case PrefixExpression n:
                var prefixExpRight = Eval(n.Right);
                if (IsError(prefixExpRight)) return prefixExpRight;
                return EvalPrefixExpression(n.Token.Literal, prefixExpRight);
            case InfixExpression n:
                var left = Eval(n.Left);
                if (IsError(left)) return left;
                var right = Eval(n.Right);
                if (IsError(right)) return right;
                return EvalInfixExpression(n.Token.Literal, left, right);
            case BlockStatement n:
                var block = EvalBlockStatement(n);
                return block;
            case ReturnStatement n:
                return new MonkeyReturn(Eval(n.Value));
            case IFExpression ifExpression:
                return EvalIfExpression(ifExpression);
        }

        return null;
    }

    private IMonkeyObject NativeBoolToBooleanObject(bool value)
    {
        return new MonkeyBoolean(value);
    }

    public IMonkeyObject? EvalProgram(List<IStatement> statements)
    {
        IMonkeyObject? obj = null;
        foreach (var stmt in statements)
        {
            obj = Eval(stmt);
            switch (obj)
            {
                case MonkeyReturn monkeyReturn:
                    return monkeyReturn.Value;
                case MonkeyError monkeyError:
                    return monkeyError;
            }

        }
        return obj;
    }

    public IMonkeyObject? EvalBlockStatement(BlockStatement blockStatement)
    {
        IMonkeyObject? obj = null;
        foreach (var stmt in blockStatement.Statements)
        {
            obj = Eval(stmt);
            if (obj != null)
            {
                if (obj.GetMonkeyObjectType() == MonkeyObjectType.Return && obj.GetMonkeyObjectType() == MonkeyObjectType.Error)
                {
                    return obj;
                }
            }
        }
        
        return obj;
    }
  
    public IMonkeyObject EvalPrefixExpression(string op, IMonkeyObject? right)
    {
        if (right == null) return new MonkeyNull();

        switch (op)
        {
            case "!":
                return EvalBangOperatorExpression(right);
            case "-":
                return EvalMinusPrefiOperatorExpression(right);
            default:
                return NewError("unknown operator:", op, right.GetMonkeyObjectType().ToString());
        }
    }

    public IMonkeyObject EvalBangOperatorExpression(IMonkeyObject right)
    {
        switch (right)
        {
            case MonkeyBoolean boolean:
                return NativeBoolToBooleanObject(!boolean.Value);
            case null:
                return NativeBoolToBooleanObject(true);
            default:
                return NativeBoolToBooleanObject(false);
        }
    }

    public IMonkeyObject EvalMinusPrefiOperatorExpression(IMonkeyObject right)
    {
        if (right.GetMonkeyObjectType() != MonkeyObjectType.Integer)
        {
            return NewError("unknown operator:", right.GetMonkeyObjectType().ToString());
        }

        var r = right as MonkeyInteger;
        if (r == null)
        {
            return new MonkeyNull();
        }

        return new MonkeyInteger(-r.Value);
    }

    public IMonkeyObject EvalInfixExpression(string op, IMonkeyObject? left, IMonkeyObject? right)
    {
        if (left == null || right == null)
        {
            return NewError("MonkeyObject is null:");
        }

        if (left.GetMonkeyObjectType() != right.GetMonkeyObjectType())
        {
            return NewError("type mismatch:", left.GetMonkeyObjectType().ToString(), op, right.GetMonkeyObjectType().ToString());
        }

        if (left.GetMonkeyObjectType() == MonkeyObjectType.Integer && right.GetMonkeyObjectType() == MonkeyObjectType.Integer)
        {
            return EvalIntegerInfixExpression(op, left, right);
        }

        switch (op)
        {
            case "==":
                return NativeBoolToBooleanObject(left.Inspect() == right.Inspect());
            case "!=":
                return NativeBoolToBooleanObject(left.Inspect() != right.Inspect());
            default:
                return new MonkeyNull();
        }
    }

    public IMonkeyObject EvalIntegerInfixExpression(string op, IMonkeyObject left, IMonkeyObject right)
    {
        var rv = right as MonkeyInteger;
        var lv = left as MonkeyInteger;

        if (lv == null || rv == null)
        {
            return NewError("MonkeyObject is not MonkeyInteger:", left.GetMonkeyObjectType().ToString(), right.GetMonkeyObjectType().ToString());
        }

        switch (op)
        {
            case "+":
                return new MonkeyInteger(lv.Value + rv.Value);
            case "-":
                return new MonkeyInteger(lv.Value - rv.Value);
            case "*":
                return new MonkeyInteger(lv.Value * rv.Value);
            case "/":
                return new MonkeyInteger(lv.Value / rv.Value);
            case "<":
                return NativeBoolToBooleanObject(lv.Value < rv.Value);
            case ">":
                return NativeBoolToBooleanObject(lv.Value > rv.Value);
            case "==":
                return NativeBoolToBooleanObject(lv.Value == rv.Value);
            case "!=":
                return NativeBoolToBooleanObject(lv.Value != rv.Value);
            default:
                return NewError("unknown operator:", left.GetMonkeyObjectType().ToString(), op, right.GetMonkeyObjectType().ToString());

        }
    }

    public IMonkeyObject? EvalIfExpression(IFExpression expression)
    {
        var condiion = Eval(expression.Condition);
        //if (IsError(condiion))
        //{
        //    return condiion;
        //}
        if (IsTruthy(condiion))
        {
            return Eval(expression.Consequence);
        }
        else if (expression.Alternative != null)
        {
            return Eval(expression.Alternative);
        }
        else
        {
            return new MonkeyNull();
        }
    }

    public bool IsTruthy(IMonkeyObject? val)
    {
        switch (val)
        {
            case MonkeyNull monkeyNull:
                return false;
            case MonkeyBoolean monkeyBoolean:
                return monkeyBoolean.Value;
            default:
                return false;//默认不是应该false吗？书上是true
        }
    }

    public MonkeyError NewError(params string[] msg)
    {
        return new MonkeyError(string.Join(" ", msg));
    }

    public bool IsError(IMonkeyObject? obj)
    {
        if (obj == null) return false;
        if (obj.GetMonkeyObjectType() != MonkeyObjectType.Error) return false;
        return true;
    }

}
