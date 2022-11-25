internal interface IMonkeyObject
{
    internal string Inspect();
    MonkeyObjectType GetMonkeyObjectType();
}

internal enum MonkeyObjectType
{
    INTEGER,
    Boolean,
    Null,
}

internal class MonkeyInteger : IMonkeyObject
{
    public MonkeyObjectType Type = MonkeyObjectType.INTEGER;

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
