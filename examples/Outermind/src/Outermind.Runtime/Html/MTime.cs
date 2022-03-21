namespace Outermind.Html;

public class MTime : MElement
{
    internal MTime(MGlobals? globals, string? dateTime = null) : base(MNodeType.Time, globals) =>
        DateTime = dateTime ?? "";

    public string DateTime { get; }

    protected internal override MNode Accept(MNodeVisitor visitor) =>
        visitor.VisitTime(this);

    public override MElement Rewrite(MGlobals globals) =>
        globals == Globals ? this : new(globals, DateTime);
}
