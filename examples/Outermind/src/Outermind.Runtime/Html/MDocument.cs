using System.Xml.Linq;

namespace Outermind.Html;

public class MDocument : MElement
{
    internal MDocument(MGlobals globals, MDocumentHead head, MElement body) : base(MNodeType.Document, globals)
    {
        Head = head;
        Body = body;
    }

    public override MNodeType NodeType => MNodeType.Document;
    public MDocumentHead Head { get; }
    public MElement Body { get; }

    protected internal override MNode Accept(MNodeVisitor visitor) =>
        visitor.VisitDocument(this);

    public MElement Rewrite(MGlobals globals, MDocumentHead head, MElement body) =>
        globals == Globals && head == Head && body == Body ? this : new(globals, head, body);

    public static MDocument Parse(string html, LoadOptions loadOptions = default) =>
        XmlReader.Read(XDocument.Load(new StringReader(html), loadOptions));

    public static MDocument Parse(Stream html, LoadOptions loadOptions = default) =>
        XmlReader.Read(XDocument.Load(html, loadOptions));

    public static async Task<MDocument> ParseAsync(Stream html, CancellationToken cancellationToken, LoadOptions loadOptions = default) =>
        XmlReader.Read(await XDocument.LoadAsync(html, loadOptions, cancellationToken));
}
