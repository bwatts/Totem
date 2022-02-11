namespace Outermind;

public class MContent : MPart
{
    internal MContent(object? value)
    {
        Value = value;
        IsNone = Value is null;
        IsPart = Value is MPart;
        IsList = Value is IReadOnlyList<object>;
    }

    public override MPartType PartType => MPartType.Content;
    public object? Value { get; }
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsNone { get; }
    public bool IsPart { get; }
    public bool IsList { get; }

    protected internal override MPart Accept(MPartVisitor visitor) =>
        visitor.VisitContent(this);

    public MContent Rewrite(object? value) =>
        value == Value ? this : new(value);

    public static readonly MContent None = new(null);
}
