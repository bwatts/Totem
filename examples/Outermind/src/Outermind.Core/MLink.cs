namespace Outermind;

public class MLink : MElement
{
    internal MLink(MGlobals? globals, string href, string hrefLanguage, string contentType, string relationship, string download)
         : base(MPartType.Link, globals)
    {
        Href = href ?? "";
        HrefLanguage = hrefLanguage ?? "";
        Relationship = relationship ?? "";
        ContentType = contentType ?? "";
        Download = download;
    }

    public string Href { get; private set; }
    public string HrefLanguage { get; private set; }
    public string ContentType { get; private set; }
    public string Relationship { get; private set; }
    public string Download { get; private set; }

    protected internal override MPart Accept(MPartVisitor visitor) =>
        visitor.VisitLink(this);

    public override MElement Rewrite(MGlobals globals) =>
        globals == Globals ? this : new(globals, Href, HrefLanguage, ContentType, Relationship, Download);
}
