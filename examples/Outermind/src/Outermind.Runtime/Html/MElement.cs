using System.Xml.Linq;

namespace Outermind.Html;

public class MElement : MNode
{
    internal MElement(MNodeType nodeType, MGlobals? globals)
    {
        NodeType = nodeType;
        Globals = globals ?? MGlobals.Default;
    }

    public override MNodeType NodeType { get; }
    public MGlobals Globals { get; }

    public XElement ToXml() =>
        new XmlWriter().ToXml(this);

    public override string ToString() =>
        ToXml().ToString();

    protected internal override MNode Accept(MNodeVisitor visitor) =>
        NodeType switch
        {
            MNodeType.Article => visitor.VisitArticle(this),
            MNodeType.Aside => visitor.VisitAside(this),
            MNodeType.Citation => visitor.VisitCitation(this),
            MNodeType.Code => visitor.VisitCode(this),
            MNodeType.Description => visitor.VisitDescription(this),
            MNodeType.DescriptionList => visitor.VisitDescriptionList(this),
            MNodeType.DescriptionTerm => visitor.VisitDescriptionTerm(this),
            MNodeType.Division => visitor.VisitDivision(this),
            MNodeType.DocumentBody => visitor.VisitDocumentBody(this),
            MNodeType.Footer => visitor.VisitFooter(this),
            MNodeType.Header => visitor.VisitHeader(this),
            MNodeType.Heading1 => visitor.VisitHeading1(this),
            MNodeType.Heading2 => visitor.VisitHeading2(this),
            MNodeType.Heading3 => visitor.VisitHeading3(this),
            MNodeType.Heading4 => visitor.VisitHeading4(this),
            MNodeType.Heading5 => visitor.VisitHeading5(this),
            MNodeType.Heading6 => visitor.VisitHeading6(this),
            MNodeType.List => visitor.VisitList(this),
            MNodeType.ListItem => visitor.VisitListItem(this),
            MNodeType.Literal => visitor.VisitLiteral(this),
            MNodeType.Main => visitor.VisitMain(this),
            MNodeType.Navigation => visitor.VisitNavigation(this),
            MNodeType.Paragraph => visitor.VisitParagraph(this),
            MNodeType.Section => visitor.VisitSection(this),
            MNodeType.Span => visitor.VisitSpan(this),
            _ => throw new NotSupportedException($"Unsupported element type {NodeType}"),
        };

    public virtual MElement Rewrite(MGlobals globals) =>
        globals == Globals ? this : new(NodeType, globals);
}
