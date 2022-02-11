namespace Outermind;

public class MTime : MElement
{
    internal MTime(MGlobals? globals, string dateTime) : base(MPartType.Time, globals) =>
        DateTime = dateTime ?? "";

    public string DateTime { get; }

    protected internal override MPart Accept(MPartVisitor visitor) =>
        visitor.VisitTime(this);

    public override MElement Rewrite(MGlobals globals) =>
        globals == Globals ? this : new(globals, DateTime);
}
