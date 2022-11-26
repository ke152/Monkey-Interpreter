internal class Evaluator
{
    public IMonkeyObject? Eval(INode? node, MonkeyEnvironment env)
    {
        if (node == null) return null;

        switch (node)
        {
            // statements
            case AtsProgram n:
                return EvalProgram(n.Statements, env);
            case BlockStatement n:
                var block = EvalBlockStatement(n, env);
                return block;
            case ExpressionStatement n:
                return Eval(n.Expression, env);
            case ReturnStatement n:
                var ret = Eval(n.Value, env);
                if (IsError(ret)) return ret;
                return new MonkeyReturn(ret);
            case LetStatement n:
                var letRet = Eval(n.Value, env);
                if (IsError(letRet)) return letRet;
                if (letRet != null) env.Set(n.Name?.TokenLiteral(), letRet);
                break;

            // expression
            case IntegerLiteral n:
                return new MonkeyInteger(n.Value);
            case BooleanExpression n:
                return NativeBoolToBooleanObject(n.Value);
            case PrefixExpression n:
                var prefixExpRight = Eval(n.Right, env);
                if (IsError(prefixExpRight)) return prefixExpRight;
                return EvalPrefixExpression(n.TokenLiteral(), prefixExpRight);
            case InfixExpression n:
                var left = Eval(n.Left, env);
                if (IsError(left)) return left;
                var right = Eval(n.Right, env);
                if (IsError(right)) return right;
                return EvalInfixExpression(n.TokenLiteral(), left, right);
            case IFExpression n:
                return EvalIfExpression(n, env);
            case Identifier n:
                return EvalIdentifier(n, env);
            case FunctionLiteral n:
                var param = n.Parameter;
                var body = n.Body;
                return new MonkeyFunction() { Body = body, Parameter = param, Env = env };
            case CallExpression callExpression:
                var func = Eval(callExpression.Function, env);
                if (IsError(func))
                    return func;
                var args = EvalExpressions(callExpression.Arguments, env);
                if (args.Count == 1 && IsError(args[0]))
                    return args[0];
                return ApplyFunction(func, args);
            case StringLiteral n:
                return new MonkeyString(n.Value);

        }

        return null;
    }

    private IMonkeyObject NativeBoolToBooleanObject(bool value)
    {
        return new MonkeyBoolean(value);
    }

    public IMonkeyObject? EvalProgram(List<IStatement> statements, MonkeyEnvironment env)
    {
        IMonkeyObject? obj = null;
        foreach (var stmt in statements)
        {
            obj = Eval(stmt, env);
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

    public IMonkeyObject? EvalBlockStatement(BlockStatement blockStatement, MonkeyEnvironment env)
    {
        IMonkeyObject? obj = null;
        foreach (var stmt in blockStatement.Statements)
        {
            obj = Eval(stmt, env);
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

        if (left.GetMonkeyObjectType() == MonkeyObjectType.String && right.GetMonkeyObjectType() == MonkeyObjectType.String)
        {
            return EvalStringInfixExpression(op, left, right);
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

    public IMonkeyObject? EvalIfExpression(IFExpression expression, MonkeyEnvironment env)
    {
        var condiion = Eval(expression.Condition, env);
        if (IsError(condiion))
        {
            return condiion;
        }
        if (IsTruthy(condiion))
        {
            return Eval(expression.Consequence, env);
        }
        else if (expression.Alternative != null)
        {
            return Eval(expression.Alternative, env);
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

    public static  MonkeyError NewError(params string[] msg)
    {
        return new MonkeyError(string.Join(" ", msg));
    }

    public bool IsError(IMonkeyObject? obj)
    {
        if (obj == null) return false;
        if (obj.GetMonkeyObjectType() != MonkeyObjectType.Error) return false;
        return true;
    }

    public IMonkeyObject? EvalIdentifier(Identifier node, MonkeyEnvironment env)
    {
        (IMonkeyObject? obj, bool ok) = env.Get(node.TokenLiteral());
        if (ok)
        {
            return obj;
        }

        if (Builtins.builtins.TryGetValue(node.Value ?? string.Empty, out MonkeyBuiltin? builtin))
        {
            return builtin;
        }

        return NewError("identifier not found: " + node.Value);
    }

    public List<IMonkeyObject?> EvalExpressions(List<IExpression?>? exps, MonkeyEnvironment environment)
    {
        var result = new List<IMonkeyObject?>();
        if (exps != null)
        {
            foreach (var e in exps)
            {
                var eval = Eval(e, environment);
                if (IsError(eval))
                {
                    return new List<IMonkeyObject?> { eval };
                }
                result.Add(eval);
            }
        }
        return result;
    }

    public IMonkeyObject? ApplyFunction(IMonkeyObject? func, List<IMonkeyObject?>? args)
    {

        switch (func)
        {
            case MonkeyFunction monkeyFunction:
                var extendedenv = ExtendFunctionEnv(monkeyFunction, args);
                var eval = Eval(monkeyFunction.Body, extendedenv);
                return UnwarpReturnValue(eval);
            case MonkeyBuiltin builtin:
                return builtin.Fn?.Invoke(args);
            default:
                return NewError($"not a function: {func?.GetMonkeyObjectType()}");
        }
    }

    public MonkeyEnvironment ExtendFunctionEnv(MonkeyFunction monkeyFunction, List<IMonkeyObject?>? args)
    {
        var env = MonkeyEnvironment.NewEnclosedEnvironment(monkeyFunction.Env);
        if (monkeyFunction.Parameter != null)
        {
            for (int i = 0; i < monkeyFunction.Parameter.Count; i++)
            {
                if (args != null && args.Count > i && args[i] != null)
                    env.Set(monkeyFunction.Parameter[i].Value, args[i]);
            }
        }
        return env;
    }

    public IMonkeyObject? UnwarpReturnValue(IMonkeyObject? obj)
    {
        if (obj is MonkeyReturn monkeyReturn)
        {
            return monkeyReturn.Value;
        }
        return obj;
    }

    public IMonkeyObject EvalStringInfixExpression(string op, IMonkeyObject left, IMonkeyObject right)
    {
        var rv = right as MonkeyString;
        var lv = left as MonkeyString;

        if (lv == null || rv == null)
        {
            return NewError("MonkeyObject is not MonkeyString:", left.GetMonkeyObjectType().ToString(), right.GetMonkeyObjectType().ToString());
        }

        if (op != "+")
        {
            return NewError("unknown operator:", op);
        }

        return new MonkeyString(lv.Value + rv.Value);
    }
}
