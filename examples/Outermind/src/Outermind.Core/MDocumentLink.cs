namespace Outermind;

public class MDocumentLink : MElement
{
    internal MDocumentLink(MGlobals? globals, string href, string hrefLanguage, string relationship, string contentType)
         : base(MPartType.DocumentLink, globals)
    {
        Href = href;
        HrefLanguage = hrefLanguage;
        Relationship = relationship;
        ContentType = contentType;
    }

    public string Href { get; }
    public string HrefLanguage { get; }
    public string Relationship { get; }
    public string ContentType { get; }

    protected internal override MPart Accept(MPartVisitor visitor) =>
        visitor.VisitDocumentLink(this);

    public override MDocumentLink Rewrite(MGlobals globals) =>
        globals == Globals ? this : new(globals, Href, HrefLanguage, Relationship, ContentType);
}
