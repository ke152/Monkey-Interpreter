internal interface IMonkeyObject
{
    internal string Inspect();
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
    public string Inspect()
    {
        return Value.ToString();
    }
}

internal class MonkeyBoolean : IMonkeyObject
{
    public MonkeyObjectType Type = MonkeyObjectType.Boolean;

    public bool Value;
    public string Inspect()
    {
        return Value.ToString();
    }
}

internal class MonkeyNull : IMonkeyObject
{
    public MonkeyObjectType Type = MonkeyObjectType.Null;

    public string Inspect()
    {
        return "null";
    }
}
