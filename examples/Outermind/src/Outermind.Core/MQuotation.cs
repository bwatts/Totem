namespace Outermind;

public class MQuotation : MElement
{
    internal MQuotation(MGlobals? globals, string cite) : base(MPartType.Quotation, globals) =>
        Cite = cite ?? "";

    public string Cite { get; }

    protected internal override MPart Accept(MPartVisitor visitor) =>
        visitor.VisitQuotation(this);

    public override MElement Rewrite(MGlobals globals) =>
        globals == Globals ? this : new(globals, Cite);
}
