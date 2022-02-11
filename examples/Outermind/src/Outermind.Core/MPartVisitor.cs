namespace Outermind;

public abstract class MPartVisitor
{
    protected virtual MPart Visit(object? value) =>
        value is MPart part ? Visit(part) : M.Content(value);

    protected virtual MPart Visit(MPart part) =>
        part.Accept(this);

    protected TPart VisitTyped<TPart>(TPart part) where TPart : MPart
    {
        var visited = Visit(part);

        try
        {
            return (TPart) visited;
        }
        catch(InvalidCastException exception)
        {
            throw new InvalidCastException($"Expected visited part {visited} to output {typeof(TPart)}", exception);
        }
    }

    protected virtual internal MPart VisitArticle(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitAside(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitCitation(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitClasses(MClasses part) =>
        part;

    protected virtual internal MPart VisitCode(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitContent(MContent part)
    {
        if(part.IsNone)
        {
            return part;
        }

        if(part.Value is MPart partValue)
        {
            return M.Content(Visit(partValue));
        }

        if(part.Value is not IReadOnlyList<object> list)
        {
            return part;
        }

        var visitedItems = default(List<object>);

        for(var i = 0; i < list.Count; i++)
        {
            var visitedItem = Visit(list[i]);

            if(visitedItems != null)
            {
                visitedItems.Add(visitedItem);
            }
            else
            {
                if(visitedItem != list[i])
                {
                    visitedItems = new List<object>();

                    for(var priorIndex = 0; priorIndex < i; priorIndex++)
                    {
                        visitedItems.Add(list[priorIndex]);
                    }

                    visitedItems.Add(visitedItem);
                }
            }
        }

        return part.Rewrite(visitedItems);
    }

    protected virtual internal MPart VisitData(MData part) =>
        part;

    protected virtual internal MPart VisitDescription(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitDescriptionList(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitDescriptionTerm(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitDocument(MDocument part) =>
        part.Rewrite(part.Globals, VisitTyped(part.Head), VisitTyped(part.Body));

    protected virtual internal MPart VisitDocumentHead(MDocumentHead part)
    {
        var visitedLinks = default(List<MDocumentLink>);

        for(var i = 0; i < part.Links.Count; i++)
        {
            var visitedLink = VisitTyped(part.Links[i]);

            if(visitedLinks != null)
            {
                visitedLinks.Add(visitedLink);
            }
            else
            {
                if(visitedLink != part.Links[i])
                {
                    visitedLinks = new List<MDocumentLink>();

                    for(var priorIndex = 0; priorIndex < i; priorIndex++)
                    {
                        visitedLinks.Add(part.Links[priorIndex]);
                    }

                    visitedLinks.Add(visitedLink);
                }
            }
        }

        return part.Rewrite(part.Globals, visitedLinks ?? part.Links);
    }

    protected virtual internal MPart VisitDocumentBody(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitDocumentLink(MDocumentLink part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected internal virtual MPart VisitExtension(MPart part) =>
        part.VisitChildren(this);

    protected virtual internal MPart VisitFooter(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitForm(MForm part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitGlobals(MGlobals part) =>
        part.Rewrite(VisitTyped(part.Classes), VisitTyped(part.Data), VisitTyped(part.Content));

    protected virtual internal MPart VisitHeader(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitHeading1(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitHeading2(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitHeading3(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitHeading4(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitHeading5(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitHeading6(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitLink(MLink part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitList(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitListItem(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitLiteral(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitMain(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitNavigation(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitParagraph(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitQuotation(MQuotation part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitSection(MElement part) =>
        part.Rewrite(VisitTyped(part.Globals));

    protected virtual internal MPart VisitTime(MTime part) =>
        part.Rewrite(VisitTyped(part.Globals));
}
