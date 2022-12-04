internal interface IMonkeyObject
{
    internal string Inspect();
    MonkeyObjectType GetMonkeyObjectType();
}

internal interface IHashKey
{
    public HashKey HashKey();
}

internal struct HashKey
{
    internal MonkeyObjectType Type;
    internal Int64 Value;
}

internal enum MonkeyObjectType
{
    Integer,
    Boolean,
    Null,
    Return,
    Error,
    Function,
    String,
    Builtin,
    Array,
    Hash,
    Quote,
}

internal class MonkeyInteger : IMonkeyObject, IHashKey
{
    public MonkeyObjectType Type = MonkeyObjectType.Integer;

    public int Value;

    public MonkeyInteger(int value)
    {
        Value = value;
    }

    public MonkeyObjectType GetMonkeyObjectType() => Type;

    public HashKey HashKey()
    {
        return new HashKey { Type = Type, Value = (Int64)Value };
    }

    public string Inspect()
    {
        return Value.ToString();
    }
}

internal class MonkeyBoolean : IMonkeyObject, IHashKey
{
    public MonkeyObjectType Type = MonkeyObjectType.Boolean;

    public bool Value;
    public MonkeyObjectType GetMonkeyObjectType() => Type;

    public MonkeyBoolean(bool value)
    {
        Value = value;
    }

    public string Inspect()
    {
        return Value.ToString();
    }

    public HashKey HashKey()
    {
        return new HashKey { Type = Type, Value = Value ? 1 : 0 };
    }
}

internal class MonkeyNull : IMonkeyObject
{
    public MonkeyObjectType Type = MonkeyObjectType.Null;
    public MonkeyObjectType GetMonkeyObjectType() => Type;

    public string Inspect()
    {
        return "null";
    }
}

internal class MonkeyReturn : IMonkeyObject
{
    public MonkeyObjectType Type = MonkeyObjectType.Return;
    public MonkeyObjectType GetMonkeyObjectType() => Type;

    public IMonkeyObject? Value;

    public MonkeyReturn(IMonkeyObject? value)
    {
        this.Value = value;
    }

    public string Inspect()
    {
        if (Value == null) return string.Empty;
        return Value.Inspect();
    }
}

class MonkeyError : IMonkeyObject
{
    public MonkeyObjectType Type = MonkeyObjectType.Error;
    public MonkeyObjectType GetMonkeyObjectType() => Type;

    public string Message;

    public MonkeyError(string m)
    {
        this.Message = m;
    }
    public string Inspect()
    {
        return "error: " + Message;
    }
}

internal class MonkeyEnvironment
{
    public Dictionary<string, IMonkeyObject?> Store = new();

    public MonkeyEnvironment? OuterEnv;

    public MonkeyEnvironment()
    {

    }

    public (IMonkeyObject?, bool) Get(string name)
    {
        if (Store.TryGetValue(name, out IMonkeyObject? obj))
        {
            return (obj, true);
        }
        else
        {
            if (OuterEnv != null)
            {
                return OuterEnv.Get(name);
            }
        }
        return (obj, false);
    }

    public IMonkeyObject? Set(string? name, IMonkeyObject? monkeyobject)
    {
        if (name == null) return null;
        Store[name] = monkeyobject;
        return monkeyobject;
    }

    public static MonkeyEnvironment NewEnvironment()
    {
        return new MonkeyEnvironment();
    }

    public static MonkeyEnvironment NewEnclosedEnvironment(MonkeyEnvironment? monkeyEnvironment)
    {
        var env = new MonkeyEnvironment();
        env.OuterEnv = monkeyEnvironment;
        return env;
    }
}

class MonkeyFunction : IMonkeyObject
{
    public MonkeyObjectType Type = MonkeyObjectType.Function;
    public MonkeyObjectType GetMonkeyObjectType() => Type;
    public List<Identifier>? Parameter;
    public BlockStatement? Body;
    public MonkeyEnvironment? Env;
    public string Inspect()
    {
        string str = string.Empty;
        if (Parameter != null)
        {
            foreach (var item in Parameter)
            {
                str += item.String();
            }
        }
        str += $"fn\r\n({string.Join(", ", str)}){{\n{Body?.String()}\n}}";
        return str;
    }
}

internal class MonkeyString : IMonkeyObject, IHashKey
{
    public MonkeyObjectType Type = MonkeyObjectType.String;
    public MonkeyObjectType GetMonkeyObjectType() => Type;

    public string Value;

    public MonkeyString(string value)
    {
        Value = value;
    }


    public string Inspect()
    {
        return Value;
    }

    public HashKey HashKey()
    {
        return new HashKey { Type = Type, Value = Value.GetHashCode() };
    }
}

class MonkeyBuiltin : IMonkeyObject
{
    public MonkeyObjectType Type = MonkeyObjectType.Builtin;
    public MonkeyObjectType GetMonkeyObjectType() => Type;

    public delegate IMonkeyObject? BuiltinFunction(List<IMonkeyObject?>? args);

    public BuiltinFunction? Fn;

    public MonkeyBuiltin(BuiltinFunction? fn)
    {
        Fn = fn;
    }

    public string Inspect()
    {
        return "builtin function";
    }
}

class MonkeyArray : IMonkeyObject
{
    public MonkeyObjectType Type = MonkeyObjectType.Array;
    public MonkeyObjectType GetMonkeyObjectType() => Type;

    public List<IMonkeyObject?> Elements;

    public MonkeyArray(List<IMonkeyObject?>? elements = null)
    {
        if (elements == null) this.Elements = new();
        else this.Elements = elements;
    }

    public string Inspect()
    {
        return $"[{string.Join(',', Elements.Select((e)=>e?.Inspect()))}]";
    }
}

internal struct HashPair
{
    public IMonkeyObject Key;
    public IMonkeyObject? Value;
}

internal class MonkeyHash : IMonkeyObject
{
    public MonkeyObjectType Type = MonkeyObjectType.Hash;
    public MonkeyObjectType GetMonkeyObjectType() => Type;

    public Dictionary<HashKey, HashPair> Pairs = new();
    public string Inspect()
    {
        var str = new List<string>();
        foreach (var item in Pairs)
        {
            str.Add($"{item.Value.Key.Inspect()}:{item.Value.Value?.Inspect()}");
        }
        return $"{{{string.Join(",", str)}}}";
    }
}

internal class MonkeyQuote : IMonkeyObject
{
    public MonkeyObjectType Type = MonkeyObjectType.Quote;
    public MonkeyObjectType GetMonkeyObjectType() => Type;

    public INode Node;
    public MonkeyQuote(INode node)
    {
        Node = node;
    }

    public string Inspect()
    {
        return $"Quote({Node?.String()})";
    }
}
