namespace Outermind.Html;

public class MQuotation : MElement
{
    internal MQuotation(MGlobals? globals, string? cite = null) : base(MNodeType.Quotation, globals) =>
        Cite = cite ?? "";

    public string Cite { get; }

    protected internal override MNode Accept(MNodeVisitor visitor) =>
        visitor.VisitQuotation(this);

    public override MElement Rewrite(MGlobals globals) =>
        globals == Globals ? this : new(globals, Cite);
}
