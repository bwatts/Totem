namespace Outermind.Html;

public class MContent : MNode
{
    internal MContent(object? value)
    {
        Value = value;
        IsNone = Value is null;
        IsElement = Value is MElement;
        IsList = Value is IReadOnlyList<object?>;
    }

    public override MNodeType NodeType => MNodeType.Content;
    public object? Value { get; }
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsNone { get; }
    public bool IsElement { get; }
    public bool IsList { get; }

    public override string? ToString() =>
        Value?.ToString();

    protected internal override MNode Accept(MNodeVisitor visitor) =>
        visitor.VisitContent(this);

    public MContent Rewrite(object? value) =>
        value == Value ? this : new(value);

    public static readonly MContent None = new(null);
}
