namespace Outermind;

public class MDocumentHead : MElement
{
    internal MDocumentHead(MGlobals? globals, string title, string baseHref, IReadOnlyList<MDocumentLink>? links = null)
        : base(MPartType.DocumentHead, globals)
    {
        Title = title ?? "";
        BaseHref = baseHref ?? "";
        Links = links ?? Array.Empty<MDocumentLink>();
    }

    public string Title { get; private set; }
    public string BaseHref { get; private set; }
    public IReadOnlyList<MDocumentLink> Links { get; private set; }

    protected internal override MPart Accept(MPartVisitor visitor) =>
        visitor.VisitDocumentHead(this);

    public MDocumentHead Rewrite(MGlobals globals, IReadOnlyList<MDocumentLink> links) =>
        globals == Globals && links == Links ? this : new(globals, Title, BaseHref, links);
}
