namespace Outermind.Html;

public abstract class MNodeVisitor
{
    protected virtual MNode Visit(MNode node) =>
        node.Accept(this);

    protected Tnode VisitTyped<Tnode>(Tnode node) where Tnode : MNode
    {
        var visited = Visit(node);

        try
        {
            return (Tnode) visited;
        }
        catch(InvalidCastException exception)
        {
            throw new InvalidCastException($"Expected visited node {visited} to output {typeof(Tnode)}", exception);
        }
    }

    protected virtual internal MNode VisitArticle(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitAside(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitCitation(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitCode(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitContent(MContent node) =>
        node.Rewrite(VisitContentValue(node.Value));

    protected virtual internal object? VisitContentValue(object? value)
    {
        if(value is MNode node)
        {
            return Visit(node);
        }

        if(value is not IReadOnlyList<object?> list)
        {
            return value;
        }

        var visitedItems = default(List<object?>);

        for(var i = 0; i < list.Count; i++)
        {
            var visitedItem = VisitContentValue(list[i]);

            if(visitedItems is not null)
            {
                visitedItems.Add(visitedItem);
            }
            else
            {
                if(visitedItem != list[i])
                {
                    visitedItems = new List<object?>();

                    for(var priorIndex = 0; priorIndex < i; priorIndex++)
                    {
                        visitedItems.Add(list[priorIndex]);
                    }

                    visitedItems.Add(visitedItem);
                }
            }
        }

        return visitedItems ?? value;
    }

    protected virtual internal MNode VisitData(MData node) =>
        node;

    protected virtual internal MNode VisitDescription(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitDescriptionList(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitDescriptionTerm(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitDivision(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitDocument(MDocument node) =>
        node.Rewrite(node.Globals, VisitTyped(node.Head), VisitTyped(node.Body));

    protected virtual internal MNode VisitDocumentHead(MDocumentHead node)
    {
        var visitedLinks = default(List<MDocumentLink>);

        for(var i = 0; i < node.Links.Count; i++)
        {
            var visitedLink = VisitTyped(node.Links[i]);

            if(visitedLinks != null)
            {
                visitedLinks.Add(visitedLink);
            }
            else
            {
                if(visitedLink != node.Links[i])
                {
                    visitedLinks = new List<MDocumentLink>();

                    for(var priorIndex = 0; priorIndex < i; priorIndex++)
                    {
                        visitedLinks.Add(node.Links[priorIndex]);
                    }

                    visitedLinks.Add(visitedLink);
                }
            }
        }

        return node.Rewrite(node.Globals, visitedLinks ?? node.Links);
    }

    protected virtual internal MNode VisitDocumentBody(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitDocumentLink(MDocumentLink node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected internal virtual MNode VisitExtension(MNode node) =>
        node.VisitChildren(this);

    protected virtual internal MNode VisitFooter(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitForm(MForm node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitGlobals(MGlobals node) =>
        node.Rewrite(VisitTyped(node.Content));

    protected virtual internal MNode VisitHeader(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitHeading1(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitHeading2(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitHeading3(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitHeading4(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitHeading5(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitHeading6(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitLink(MLink node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitList(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitListItem(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitLiteral(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitMain(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitNavigation(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitParagraph(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitQuotation(MQuotation node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitSection(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitSpan(MElement node) =>
        node.Rewrite(VisitTyped(node.Globals));

    protected virtual internal MNode VisitTime(MTime node) =>
        node.Rewrite(VisitTyped(node.Globals));
}
