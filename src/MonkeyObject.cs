internal interface IMonkeyObject
{
    internal string Inspect();
    MonkeyObjectType GetMonkeyObjectType();
}

internal enum MonkeyObjectType
{
    Integer,
    Boolean,
    Null,
    Return,
}

internal class MonkeyInteger : IMonkeyObject
{
    public MonkeyObjectType Type = MonkeyObjectType.Integer;

    public int Value;

    public MonkeyInteger(int value)
    {
        Value = value;
    }

    public MonkeyObjectType GetMonkeyObjectType() => Type;

    public string Inspect()
    {
        return Value.ToString();
    }
}

internal class MonkeyBoolean : IMonkeyObject
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
        return Value.Inspect();
    }
}
