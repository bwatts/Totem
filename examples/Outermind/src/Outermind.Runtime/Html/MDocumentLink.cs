namespace Outermind.Html;

public class MDocumentLink : MElement
{
    internal MDocumentLink(MGlobals? globals, string? href = null, string? hrefLanguage = null, string? relationship = null, string? contentType = null)
         : base(MNodeType.DocumentLink, globals)
    {
        Href = href ?? "";
        HrefLanguage = hrefLanguage ?? "";
        Relationship = relationship ?? "";
        ContentType = contentType ?? "";
    }

    public string Href { get; }
    public string HrefLanguage { get; }
    public string Relationship { get; }
    public string ContentType { get; }

    protected internal override MNode Accept(MNodeVisitor visitor) =>
        visitor.VisitDocumentLink(this);

    public override MDocumentLink Rewrite(MGlobals globals) =>
        globals == Globals ? this : new(globals, Href, HrefLanguage, Relationship, ContentType);
}
