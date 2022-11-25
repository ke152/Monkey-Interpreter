internal class Evaluator
{
    public static IMonkeyObject? Eval(INode? node)
    {
        if (node == null) return null;

        switch (node)
        {
            case AtsProgram p:
                return EvalStatements(p.Statements);
            case ExpressionStatement e:
                return Eval(e);
            case IntegerLiteral l:
                return new MonkeyInteger(l.Value);
            case BooleanExpression e:
                return NativeBoolToBooleanObject(e.Value);
            case PrefixExpression e:
                var obj = Eval(e);
                return EvalPrefixExpression(e.Operator, obj);
            case InfixExpression e:
                var left = Eval(e.Left);
                var right = Eval(e.Right);
                return EvalInfixExpression(e.Operator, left, right);
        }

        return null;
    }

    private static IMonkeyObject NativeBoolToBooleanObject(bool value)
    {
        return new MonkeyBoolean(value);
    }

    public static IMonkeyObject? EvalStatements(List<IStatement> statements)
    {
        IMonkeyObject? obj = null;
        foreach (var stmt in statements)
        {
            obj = Eval(stmt);
            //switch (obj)
            //{
            //    case MonkeyReturn monkeyReturn:
            //        return monkeyReturn.Value;
            //    case MonkeyError monkeyError:
            //        return monkeyError;
            //}

        }
        return obj;
    }

    public static IMonkeyObject EvalPrefixExpression(string op, IMonkeyObject? right)
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

    public static IMonkeyObject EvalBangOperatorExpression(IMonkeyObject right)
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

    public static IMonkeyObject EvalMinusPrefiOperatorExpression(IMonkeyObject right)
    {
        if (right.GetMonkeyObjectType() != MonkeyObjectType.INTEGER)
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

    public static IMonkeyObject EvalInfixExpression(string op, IMonkeyObject? left, IMonkeyObject? right)
    {
        if (left == null || right == null)
        {
            return new MonkeyNull();
        }

        if (left.GetMonkeyObjectType() == MonkeyObjectType.INTEGER && right.GetMonkeyObjectType() == MonkeyObjectType.INTEGER)
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

    public static IMonkeyObject EvalIntegerInfixExpression(string op, IMonkeyObject left, IMonkeyObject right)
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
}
