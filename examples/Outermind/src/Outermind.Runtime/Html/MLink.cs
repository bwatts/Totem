namespace Outermind.Html;

public class MLink : MElement
{
    internal MLink(MGlobals? globals, string? href = null, string? hrefLanguage = null, string? contentType = null, string? relationship = null, string? download = null)
         : base(MNodeType.Link, globals)
    {
        Href = href ?? "";
        HrefLanguage = hrefLanguage ?? "";
        Relationship = relationship ?? "";
        ContentType = contentType ?? "";
        Download = download ?? "";
    }

    public string Href { get; private set; }
    public string HrefLanguage { get; private set; }
    public string ContentType { get; private set; }
    public string Relationship { get; private set; }
    public string Download { get; private set; }

    protected internal override MNode Accept(MNodeVisitor visitor) =>
        visitor.VisitLink(this);

    public override MElement Rewrite(MGlobals globals) =>
        globals == Globals ? this : new(globals, Href, HrefLanguage, ContentType, Relationship, Download);
}
