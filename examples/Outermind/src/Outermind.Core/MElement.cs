namespace Outermind;

public class MElement : MPart
{
    internal MElement(MPartType partType, MGlobals? globals = null)
    {
        PartType = partType;
        Globals = globals ?? MGlobals.Default;
    }

    public override MPartType PartType { get; }
    public MGlobals Globals { get; }

    protected internal override MPart Accept(MPartVisitor visitor) =>
        PartType switch
        {
            MPartType.Article => visitor.VisitArticle(this),
            MPartType.Aside => visitor.VisitAside(this),
            MPartType.Citation => visitor.VisitCitation(this),
            MPartType.Code => visitor.VisitCode(this),
            MPartType.Description => visitor.VisitDescription(this),
            MPartType.DescriptionList => visitor.VisitDescriptionList(this),
            MPartType.DescriptionTerm => visitor.VisitDescriptionTerm(this),
            MPartType.DocumentBody => visitor.VisitDocumentBody(this),
            MPartType.Footer => visitor.VisitFooter(this),
            MPartType.Header => visitor.VisitHeader(this),
            MPartType.Heading1 => visitor.VisitHeading1(this),
            MPartType.Heading2 => visitor.VisitHeading2(this),
            MPartType.Heading3 => visitor.VisitHeading3(this),
            MPartType.Heading4 => visitor.VisitHeading4(this),
            MPartType.Heading5 => visitor.VisitHeading5(this),
            MPartType.Heading6 => visitor.VisitHeading6(this),
            MPartType.List => visitor.VisitList(this),
            MPartType.ListItem => visitor.VisitListItem(this),
            MPartType.Literal => visitor.VisitLiteral(this),
            MPartType.Main => visitor.VisitMain(this),
            MPartType.Navigation => visitor.VisitNavigation(this),
            MPartType.Paragraph => visitor.VisitParagraph(this),
            MPartType.Section => visitor.VisitSection(this),
            _ => throw new NotSupportedException($"Unsupported element type {PartType}"),
        };

    public virtual MElement Rewrite(MGlobals globals) =>
        globals == Globals ? this : new(PartType, globals);
}
