internal class Builtins
{
    public static readonly Dictionary<string, MonkeyBuiltin> builtins = new()
    {
        { "len", new MonkeyBuiltin(BuiltinFunctionLen) },
        { "first", new MonkeyBuiltin(BuiltinFunctionFirst) },
        { "last", new MonkeyBuiltin(BuiltinFunctionLast) },
        { "rest", new MonkeyBuiltin(BuiltinFunctionRest) },
        { "push", new MonkeyBuiltin(BuiltinFunctionPush) },
    };

    public static IMonkeyObject? BuiltinFunctionLen(List<IMonkeyObject?>? args)
    {
        if (args == null) return Evaluator.NewError("null arguments");
        if (args.Count != 1) return Evaluator.NewError($"wrong number of arguments. got={args.Count}, want=1");

        switch (args[0])
        {
            case MonkeyArray obj:
                return new MonkeyArray(obj.Elements);
            case MonkeyString obj:
                return new MonkeyInteger(obj.Value.Length);
            default:
                return Evaluator.NewError($"argument to 'len' not support, got={args[0]?.GetMonkeyObjectType()}");
        };
    }

    public static IMonkeyObject? BuiltinFunctionFirst(List<IMonkeyObject?>? args)
    {
        if (args == null) return Evaluator.NewError("null arguments");
        if (args.Count != 1) return Evaluator.NewError($"wrong number of arguments. got={args.Count}, want=1");

        switch (args[0])
        {
            case MonkeyArray obj:
                if (obj.Elements.Count == 0) return new MonkeyNull();
                return obj.Elements[0];
            default:
                return Evaluator.NewError($"argument to 'first' must be array, got={args[0]?.GetMonkeyObjectType()}");
        };
    }

    public static IMonkeyObject? BuiltinFunctionLast(List<IMonkeyObject?>? args)
    {
        if (args == null) return Evaluator.NewError("null arguments");
        if (args.Count != 1) return Evaluator.NewError($"wrong number of arguments. got={args.Count}, want=1");

        switch (args[0])
        {
            case MonkeyArray obj:
                if (obj.Elements.Count == 0) return new MonkeyNull();
                return obj.Elements.Last();
            default:
                return Evaluator.NewError($"argument to 'last' must be array, got={args[0]?.GetMonkeyObjectType()}");
        };
    }

    public static IMonkeyObject? BuiltinFunctionRest(List<IMonkeyObject?>? args)
    {
        if (args == null) return Evaluator.NewError("null arguments");
        if (args.Count != 1) return Evaluator.NewError($"wrong number of arguments. got={args.Count}, want=1");

        switch (args[0])
        {
            case MonkeyArray obj:
                if (obj.Elements.Count == 0) return new MonkeyNull();
                return new MonkeyArray(obj.Elements.Skip(1).ToList());
            default:
                return Evaluator.NewError($"argument to 'rest' must be array, got={args[0]?.GetMonkeyObjectType()}");
        };
    }

    public static IMonkeyObject? BuiltinFunctionPush(List<IMonkeyObject?>? args)
    {
        if (args == null) return Evaluator.NewError("null arguments");
        if (args.Count != 2) return Evaluator.NewError($"wrong number of arguments. got={args.Count}, want=2");

        switch (args[0])
        {
            case MonkeyArray obj:
                if (obj.Elements.Count == 0) return new MonkeyNull();
                var ret = new MonkeyArray(obj.Elements.ToList());
                ret.Elements.Add(args[1]);
                return ret;
            default:
                return Evaluator.NewError($"argument to 'push' must be array, got={args[0]?.GetMonkeyObjectType()}");
        };
    }
}
