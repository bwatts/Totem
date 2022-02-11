namespace Outermind;

public class MDocument : MElement
{
    internal MDocument(MGlobals? globals, MDocumentHead head, MElement body) : base(MPartType.Document, globals)
    {
        Head = head;
        Body = body;
    }

    public override MPartType PartType => MPartType.Document;
    public MDocumentHead Head { get; }
    public MElement Body { get; }

    protected internal override MPart Accept(MPartVisitor visitor) =>
        visitor.VisitDocument(this);

    public MElement Rewrite(MGlobals globals, MDocumentHead head, MElement body) =>
        globals == Globals && head == Head && body == Body ? this : new(globals, head, body);
}
