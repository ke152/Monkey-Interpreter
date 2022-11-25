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
                var obj = Eval(n);
                return EvalPrefixExpression(n.Operator, obj);
            case InfixExpression n:
                var left = Eval(n.Left);
                var right = Eval(n.Right);
                return EvalInfixExpression(n.Operator, left, right);
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
                //case MonkeyError monkeyError:
                //    return monkeyError;
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
            if (obj != null && obj.GetMonkeyObjectType() == MonkeyObjectType.Return)
            {
                return obj;
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
                return new MonkeyNull();
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
            return new MonkeyNull();
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
            return new MonkeyNull();
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
            return new MonkeyNull();
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
                return new MonkeyNull();

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
}
