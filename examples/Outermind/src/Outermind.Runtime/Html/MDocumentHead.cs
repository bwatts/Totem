namespace Outermind.Html;

public class MDocumentHead : MElement
{
    internal MDocumentHead(MGlobals? globals, string? title = null, string? baseHref = null, IReadOnlyList<MDocumentLink>? links = null)
        : base(MNodeType.DocumentHead, globals)
    {
        Title = title ?? "";
        BaseHref = baseHref ?? "";
        Links = links ?? Array.Empty<MDocumentLink>();
    }

    public string Title { get; }
    public string BaseHref { get; }
    public IReadOnlyList<MDocumentLink> Links { get; }

    protected internal override MNode Accept(MNodeVisitor visitor) =>
        visitor.VisitDocumentHead(this);

    public MDocumentHead Rewrite(MGlobals globals, IReadOnlyList<MDocumentLink> links) =>
        globals == Globals && links == Links ? this : new(globals, Title, BaseHref, links);
}
