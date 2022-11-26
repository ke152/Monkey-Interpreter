internal class Builtins
{
    public static readonly Dictionary<string, MonkeyBuiltin> builtins = new()
    {
        { "len", new MonkeyBuiltin(BuiltinFunctionLen) },
    };

    public static IMonkeyObject BuiltinFunctionLen(List<IMonkeyObject?>? args)
    {
        if (args == null) return Evaluator.NewError("null arguments");
        if (args.Count != 1) return Evaluator.NewError($"wrong number of arguments. got={args.Count}, want=1");

        var arg = args[0] as MonkeyString;
        if (arg == null) return Evaluator.NewError($"argument to 'len' not support, got={args[0]?.GetMonkeyObjectType()}");

        return new MonkeyInteger(arg.Value.Length);
    }
}
